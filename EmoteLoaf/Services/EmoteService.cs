using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Disqord.Bot;
using Disqord.Bot.Hosting;
using EmoteLoaf.Common.Entities;
using EmoteLoaf.Common.Results;
using EmoteLoaf.FileType;
using ImageMagick;
using Microsoft.Extensions.Logging;

namespace EmoteLoaf.Services
{
    public class EmoteService : DiscordBotService
    {
        private readonly ILogger<EmoteService> _logger;
        private readonly HttpClient _httpClient;
        private readonly FileTypeGuesser _fileTypeGuesser;
        
        public EmoteService(HttpClient httpClient, FileTypeGuesser fileTypeGuesser, ILogger<EmoteService> logger, DiscordBotBase bot) : base(logger, bot)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.Clear();
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Global.BotName, Global.BotVersion));
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue($"({Global.BotRepo})"));
            _fileTypeGuesser = fileTypeGuesser;
            _logger = logger;
        }
        
        public async Task<EmoteFetchResult> FetchEmoteAsync(string url)
        {
            // Get headers
            var request = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

            // Workaround if server doesn't support HEAD requests
            if (request.StatusCode == HttpStatusCode.MethodNotAllowed)
                request = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            // Check media type
            string mediaType;

            if (request.Content.Headers.ContentType != null)
            {
                mediaType = request.Content.Headers.ContentType.MediaType;
                
                if (!Global.ValidMediaTypes.Contains(mediaType))
                    return new EmoteFetchResult.Failed("The provided file format is not supported.");
            }
            else
                return new EmoteFetchResult.Failed("The server responded with invalid headers.");

            // Check file size
            int maxSize = Global.MaxFileSize;
            
            if (request.Content.Headers.ContentLength.HasValue)
            {
                var val = request.Content.Headers.ContentLength.Value;

                if (val <= Global.MaxFileSize)
                {
                    // val will always be an int here
                    maxSize = (int)val;
                }
                else 
                    return new EmoteFetchResult.Failed("Content length exceeds the 16MiB limit.");
            }
            
            // Fetch file
            const int bufferSize = 4096;
            var ms = new MemoryStream();

            await using (var stream = await _httpClient.GetStreamAsync(url))
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                int size = 0;

                while ((count = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    size += count;

                    if (size > maxSize)
                        return new EmoteFetchResult.Failed("File size exceeds the 16MiB limit.");

                    await ms.WriteAsync(buffer, 0, count);
                }
            }

            var fileType = _fileTypeGuesser.GuessFileType(ms);
            
            if (fileType.MediaType != mediaType)
                return new EmoteFetchResult.Failed("The server provided incorrect headers.");
            
            if (mediaType == "application/zip")
            {
                var extractedFiles = new List<EmoteFile>();
                
                using (var archive = new ZipArchive(ms))
                {
                    var fileCount = 0;
                    
                    foreach (var entry in archive.Entries)
                    {
                        // 50 is max that can be added at a time due to discord ratelimits.
                        if (fileCount == 50)
                            break;

                        var ext = Global.ValidExtensions.FirstOrDefault(x => entry.Name.EndsWith($".{x}"));
                        
                        if (ext == null)
                            continue;

                        var extractedFile = new MemoryStream();
                        
                        await using (var extracted = entry.Open())
                        {
                            byte[] buffer = new byte[bufferSize];
                            int count;
                            int size = 0;

                            while ((count = await extracted.ReadAsync(buffer, 0, buffer.Length)) != 0)
                            {
                                size += count;

                                if (size > Global.MaxFileSize)
                                {
                                    _logger.LogDebug($"Skipping {entry.Name}: File too large.");
                                    continue;
                                }

                                await extractedFile.WriteAsync(buffer, 0, count);
                            }
                        }

                        var extractedFileType = _fileTypeGuesser.GuessFileType(extractedFile);

                        if (extractedFileType.Extension != "zip" && extractedFileType.Extension != "unknown")
                        {
                            var isGif = extractedFileType.Extension == "gif";
                            
                            extractedFiles.Add(
                                new EmoteFile(await ResizeToEmoteAsync(extractedFile, isGif), 
                                    extractedFileType.MediaType, Utils.FormatEmoteName(entry.Name)));
                            
                            fileCount++;
                        }
                        else 
                            _logger.LogDebug($"Skipping {entry.Name}: Not a valid or supported image.");
                    }
                }

                return new EmoteFetchResult.Multiple(extractedFiles.ToArray());
            }

            var file = new EmoteFile(await ResizeToEmoteAsync(ms, fileType.Extension == "gif"), mediaType);

            return new EmoteFetchResult.Single(file);
        }

        public async Task<MemoryStream> ResizeToEmoteAsync(MemoryStream stream, bool isGif = false)
        {
            if (stream.Length <= Global.MaxEmoteSize)
                return stream;

            int resolution = 128;

            if (isGif)
            {
                
            }

            using (var img = new MagickImage(stream))
            {
                while (true)
                {
                    img.Resize(resolution, resolution);

                    if (img.ToByteArray().Length <= Global.MaxEmoteSize || resolution <= 32)
                    {
                        stream.SetLength(0);
                        await img.WriteAsync(stream);

                        return stream;
                    }

                    resolution = resolution / 2;
                }
            }
        }

        public async Task<MemoryStream> ConvertToGif(MemoryStream stream)
        {
            using (var img = new MagickImage(stream))
            {
                img.Format = MagickFormat.Gif;
                
                stream.SetLength(0);
                await img.WriteAsync(stream);

                return stream;
            }
        }
    }
}
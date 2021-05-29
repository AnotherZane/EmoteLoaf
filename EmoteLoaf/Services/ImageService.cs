using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using Disqord.Bot;
using Disqord.Bot.Hosting;
using ImageMagick;
using Microsoft.Extensions.Logging;
using MimeGuesser;

namespace EmoteLoaf.Services
{
    public class ImageService : DiscordBotService
    {
        private static readonly int MAX_EMOTE_SIZE = 256 * (int)Math.Pow(2, 10);
        private static readonly int MAX_IMAGE_SIZE = 16 *  (int)Math.Pow(2, 20);
        
        private readonly ILogger<ImageService> _logger;
        private readonly HttpClient _httpClient;
        private readonly FileTypeGuesser _fileTypeGuesser;

        private readonly List<string> _validContentTypes;

        public ImageService(HttpClient httpClient, FileTypeGuesser fileTypeGuesser, ILogger<ImageService> logger, DiscordBotBase bot) : base(logger, bot)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.Clear();
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Global.BotName, Global.BotVersion));
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue($"({Global.BotRepo})"));
            _fileTypeGuesser = fileTypeGuesser;
            _logger = logger;
            _validContentTypes = new List<string>(new[] {"image/png", "image/jpeg", "image/gif", "image/webp"});
        }
        
        public async Task<byte[]> FetchEmoteAsync(string url)
        {
            // Check headers
            var request = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

            // Workaround if server doesn't support HEAD requests
            if (request.StatusCode == HttpStatusCode.MethodNotAllowed)
                request = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            
            var contentType = request.Content.Headers.ContentType;

            if (contentType is not null)
            {
                if (_validContentTypes.Contains(contentType.MediaType)) 
                    throw new Exception("The provided file format is not supported.");
            }
            else
                throw new HttpRequestException("The server responded with invalid headers.");
            
            // Fetch file
            const int bufferSize = 4096;
            byte[] responseBytes;
            long maxSize = MAX_IMAGE_SIZE;
            
            if (request.Content.Headers.ContentLength.HasValue)
            {
                var val = request.Content.Headers.ContentLength.Value;

                if (val <= MAX_IMAGE_SIZE)
                    maxSize = val;
                else 
                    throw new HttpRequestException("Content length exceeds the 16MiB limit.");
            }
            
            await using (var stream = await _httpClient.GetStreamAsync(url))
            {
                await using (MemoryStream ms = new MemoryStream())
                {
                    byte[] buffer = new byte[bufferSize];
                    int count;
                    long size = 0;

                    while ((count = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        size += count;

                        if (size > maxSize)
                            throw new Exception("File size exceeds the 16MiB limit.");
                                
                        await ms.WriteAsync(buffer, 0, count);
                    }
                    responseBytes = ms.ToArray();
                }
            }

            var mime = _fileTypeGuesser.GuessMimeType(responseBytes);
            
            return ResizeToEmote(responseBytes);
        }

        public byte[] ResizeToEmote(byte[] bytes)
        {
            if (bytes.Length <= MAX_EMOTE_SIZE)
                return bytes;

            int resolution = 128;

            using var img = new MagickImage(bytes);

            while (true)
            {
                img.Resize(resolution, resolution);

                if (img.ToByteArray().Length <= MAX_EMOTE_SIZE || resolution <= 32)
                    return img.ToByteArray();
                
                resolution = resolution / 2;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;
using EmoteLoaf.Commands.Checks.Parameters;
using EmoteLoaf.Common.Results;
using EmoteLoaf.Extensions;
using EmoteLoaf.FileType;
using EmoteLoaf.Services;
using Qmmands;

namespace EmoteLoaf.Commands.Modules
{
    [Name("Emote")]
    [Description("Emote management")]
    [RequireBotGuildPermissions(Permission.ManageEmojis)]
    [RequireAuthorGuildPermissions(Permission.ManageEmojis)]
    public class EmoteModule : DiscordGuildModuleBase
    {
        public EmoteService EmoteService { get; set; }
        public FileTypeGuesser FileTypeGuesser { get; set; }
        
        [Name("Add Emote")]
        [Command("add")]
        [Description("Add a new emote")]
        public async ValueTask AddEmoteAsync(ICustomEmoji emoji)
            => await AddEmoteAsync(emoji.Name, Discord.Cdn.GetCustomEmojiUrl(emoji.Id, emoji.IsAnimated, 128));
        
        [Name("Add Emote")]
        [Command("add")]
        [Description("Add a new emote")]
        public async ValueTask AddEmoteAsync([Regex(Global.EmoteNameRegex)] string name)
        {
            if (Context.Message.Attachments.Count > 0)
            {
                var attachment = Context.Message.Attachments.First();

                if (attachment.ContentType != null && !Global.ValidMediaTypes.Contains(attachment.ContentType))
                {
                    await Reply("Attachment file format is not supported.");
                    return;
                }

                if (attachment.FileSize > Global.MaxFileSize)
                {
                    await Reply("Attachment size exceeds the 16MiB limit.");
                    return;
                }

                await AddEmoteAsync(name, attachment.Url);
            }
            else
            {
                //Todo: Reply with command help
                await Reply("todo help");
            }
        }
        
        [Name("Add Emote")]
        [Command("add")]
        [Description("Add a new emote")]
        public async ValueTask AddEmoteAsync()
        {
            if (Context.Message.Attachments.Count > 0)
            {
                var attachment = Context.Message.Attachments.First();

                if (attachment.ContentType != null && !Global.ValidMediaTypes.Contains(attachment.ContentType))
                {
                    await Reply("Attachment file format is not supported.");
                    return;
                }
                
                if (attachment.FileSize > Global.MaxFileSize)
                {
                    await Reply("Attachment size exceeds the 16MiB limit.");
                    return;
                }

                await AddEmoteAsync(Utils.FormatEmoteName(attachment.FileName), attachment.Url);
            }
            else
            {
                //Todo: Reply with command help
                await Reply("todo help");
            }
        }
        
        [Name("Add Emote")]
        [Command("add")]
        [Description("Add a new emote")]
        public async ValueTask AddEmoteAsync([Regex(Global.EmoteNameRegex)] string name, ICustomEmoji emoji)
            => await AddEmoteAsync(name, Discord.Cdn.GetCustomEmojiUrl(emoji.Id, emoji.IsAnimated, 128));
        
        [Name("Add Emote")]
        [Command("add")]
        [Description("Add a new emote")]
        public async ValueTask AddEmoteAsync(IList<ICustomEmoji> emojis)
        {
            try
            {
                var maxCount = Context.Guild.MaxEmoteCount();

                var animatedCount = Context.Guild.Emojis.Values.Count(x => x.IsAnimated);
                var normalCount = Context.Guild.Emojis.Values.Count(x => !x.IsAnimated);

                if (normalCount >= maxCount && animatedCount >= maxCount)
                {
                    await Reply("No emote slots available. Consider removing some emotes.");
                    return;
                }

                var normal = new List<string>();
                var animated = new List<string>();
                
                foreach (var emoji in emojis)
                {
                    EmoteFetchResult fetchResult;
                
                    await using (Context.BeginYield())
                    {
                        fetchResult = await EmoteService.FetchEmoteAsync(Discord.Cdn.GetCustomEmojiUrl(emoji.Id, emoji.IsAnimated, 128));
                    }
                    
                    if (fetchResult is EmoteFetchResult.Failed failed)
                    {
                        await Reply(failed.Message);
                    }
                    else if (fetchResult is EmoteFetchResult.Single single)
                    {
                        var options = new DefaultRestRequestOptions
                        {
                            Reason = $"Added by {Context.Author.Tag} ({Context.Author.Id})"
                        };

                        IGuildEmoji emote;
                        bool asGif = false;
                    
                        if (single.File.MediaType != "image/gif" && normalCount >= maxCount)
                        {
                            var newImage = await EmoteService.ConvertToGif(single.File.Stream);
                            emote = await Context.Guild.CreateEmojiAsync(emoji.Name, newImage, options: options);

                            asGif = true;
                        }
                        else
                            emote = await Context.Guild.CreateEmojiAsync(emoji.Name, single.File.Stream, options: options);
                        
                        if (asGif)
                            animated.Add(emote.Tag);
                        else
                            normal.Add(emote.Tag);
                    }
                }

                var reply = $"Emote(s) {string.Join(", ", normal)} were successfully created.";

                if (animated.Count > 0)
                    reply += $" Emote(s) {string.Join(", ", animated)} were successfully created as GIFs.";
                
                await Reply(reply);
            }
            catch (Exception ex)
            {
                await Response(new LocalMessage().WithEmbed(new LocalEmbed()
                    .WithDescription($"{ex.Source}:{ex.Message}")));
                
                Console.WriteLine(ex);
            }
        }
        
        [Name("Add Emote")]
        [Command("add")]
        [Description("Add a new emote")]
        public async ValueTask AddEmoteAsync([Regex(Global.EmoteNameRegex)]string name, [RequireUri]string url)
        {
            try
            {
                var maxCount = Context.Guild.MaxEmoteCount();

                var animatedCount = Context.Guild.Emojis.Values.Count(x => x.IsAnimated);
                var normalCount = Context.Guild.Emojis.Values.Count(x => !x.IsAnimated);

                if (normalCount >= maxCount && animatedCount >= maxCount)
                {
                    await Reply("No emote slots available. Consider removing some emotes.");
                    return;
                }

                EmoteFetchResult fetchResult;
                
                await using (Context.BeginYield())
                {
                    fetchResult = await EmoteService.FetchEmoteAsync(url);
                }

                if (fetchResult is EmoteFetchResult.Failed failed)
                {
                    await Reply(failed.Message);
                }
                else if (fetchResult is EmoteFetchResult.Single single)
                {
                    var options = new DefaultRestRequestOptions
                    {
                        Reason = $"Added by {Context.Author.Tag} ({Context.Author.Id})"
                    };

                    IGuildEmoji emote;
                    bool asGif = false;
                    
                    if (single.File.MediaType != "image/gif" && normalCount >= maxCount)
                    {
                        var newImage = await EmoteService.ConvertToGif(single.File.Stream);
                        emote = await Context.Guild.CreateEmojiAsync(name, newImage, options: options);

                        asGif = true;
                    }
                    else
                        emote = await Context.Guild.CreateEmojiAsync(name, single.File.Stream, options: options);

                    await Reply($"Emote {emote.Tag} was successfully created{(asGif ? " as a GIF": "")}.");
                }
                else if (fetchResult is EmoteFetchResult.Multiple multiple)
                {
                    var options = new DefaultRestRequestOptions
                    {
                        Reason = $"Added by {Context.Author.Tag} ({Context.Author.Id})"
                    };

                    var emotes = new List<string>();

                    foreach (var file in multiple.Files)
                    {
                        var emote = await Context.Guild.CreateEmojiAsync(file.Name, file.Stream, options: options);
                        emotes.Add(emote.Tag);
                    }

                    await Reply($"Emotes {string.Join(", ", emotes)} were successfully created.");
                }
            }
            catch (Exception ex)
            {
                await Response(new LocalMessage().WithEmbed(new LocalEmbed()
                        .WithDescription($"{ex.Source}:{ex.Message}")));
                
                Console.WriteLine(ex);
            }
        }
    }
}
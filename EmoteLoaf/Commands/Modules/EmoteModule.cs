using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;
using EmoteLoaf.Commands.Checks.Parameters;
using EmoteLoaf.Services;
using MimeGuesser;
using Qmmands;

namespace EmoteLoaf.Commands.Modules
{
    [Name("Emote")]
    [Description("Emote management")]
    public class EmoteModule : DiscordGuildModuleBase
    {
        public ImageService ImageService { get; set; }
        public FileTypeGuesser FileTypeGuesser { get; set; }

        [Name("Add Emote")]
        [Command("add")]
        [Description("Add a new emote")]
        public async ValueTask AddEmoteAsync([Regex(Global.EmoteNameRegex)] string name, ICustomEmoji emoji)
            => await AddEmoteAsync(name, Discord.Cdn.GetCustomEmojiUrl(emoji.Id, emoji.IsAnimated, 128));
        
        [Name("Add Emote")]
        [Command("add")]
        [Description("Add a new emote")]
        public async ValueTask AddEmoteAsync(ICustomEmoji emoji)
            => await AddEmoteAsync(emoji.Name, Discord.Cdn.GetCustomEmojiUrl(emoji.Id, emoji.IsAnimated, 128));
        
        [Name("Add Emote")]
        [Command("add")]
        [Description("Add a new emote")]
        public async ValueTask AddEmoteAsync([Regex(Global.EmoteNameRegex)]string name, [RequireUri]string url)
        {
            try
            {
                var emote = await ImageService.FetchEmoteAsync(url);

                var mimeType = FileTypeGuesser.GuessMimeType(emote);
                
                using (var attachment = new LocalAttachment(emote, $"test{mimeType.Extension}"))
                {
                    await Response(new LocalMessageBuilder().WithContent($"Mime: {mimeType.Name}").WithAttachments(attachment).Build());
                }
            }
            catch (Exception ex)
            {
                await Response(new LocalMessageBuilder().WithEmbed(new LocalEmbedBuilder()
                        .WithDescription($"{ex.Source}:{ex.Message}")).Build());
                
                Console.WriteLine(ex);
            }
        }
        
        [Name("Add Emote")]
        [Command("add")]
        [Description("Add a new emote")]
        public async ValueTask AddEmoteAsync([Regex(Global.EmoteNameRegex)]string name)
        {
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Rest;
using EmoteLoaf.Extensions;
using Humanizer;
using Qmmands;

namespace EmoteLoaf.Commands.Modules
{
    [Name("Information")]
    [Description("General bot information")]
    public class InformationModule : DiscordModuleBase
    {
        
        [Name("Ping")]
        [Command("ping")]
        [Description("Get my connection latency")]
        [Cooldown(1, 3, CooldownMeasure.Seconds, CooldownBucketType.Channel)]
        public async Task GetPingAsync()
        {
            var latency = DateTimeOffset.UtcNow - Context.Message.CreatedAt;

            var sw = Stopwatch.StartNew();
            var message = await Response("🏓 Ping?");
            sw.Stop();

            await message.ModifyAsync(m =>
                m.Content = "🏓 Pong!\n\n" +
                            $"Gateway: `{latency.Milliseconds:#,##0}ms`\n" +
                            $"RTT: `{sw.ElapsedMilliseconds:#,##0}ms`"
            );
        }

        [Name("Invite")]
        [Command("invite")]
        [Description("Get my invite link")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.Channel)]
        public DiscordCommandResult GetInvite()
            => Response(new LocalEmbedBuilder()
                .WithDefaultColor()
                .WithAuthor(Context.Bot.CurrentUser)
                .WithDescription(
                    Utils.MarkdownLink("Click here to invite me to your server", 
                        $"https://discord.com/oauth2/authorize?client_id={Context.Bot.CurrentUser.Id}" +
                        $"&scope=applications.commands%20bot&permissions={(ulong)Global.InvitePermissions}", 
                        $"Invite {Context.Bot.CurrentUser.Name}")));
        
        [Name("Information")]
        [Command("info", "information")]
        [Description("Information about me")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.Channel)]
        public async ValueTask<DiscordCommandResult> GetInfoAsync()
        {
            var owners = new List<string>();

            foreach (var id in Context.Bot.OwnerIds)
            {
                IUser user = Context.Bot.GetUser(id) ?? await Context.Bot.FetchUserAsync(id);
                
                owners.Add($"`{user.Tag}`");
            }

            using var process = Process.GetCurrentProcess();
            var uptime = (DateTime.Now - process.StartTime).Humanize();

            return Response(new LocalEmbedBuilder()
                .WithDefaultColor()
                .WithTitle(Context.Bot.CurrentUser.Name)
                .WithThumbnailUrl(Context.Bot.CurrentUser.GetAvatarUrl())
                .WithDescription($"{Context.Bot.CurrentUser.Name} provides useful commands to help you manage custom server emotes effortlessly.")
                .AddInlineField("Owners", string.Join("\n", owners))
                .AddInlineField("Uptime", uptime)
                .AddBlankInlineField()
                .AddInlineField("Source Code", Utils.MarkdownLink("Github", Global.BotRepo))
                .AddInlineField("Library", Utils.MarkdownLink($"Disqord {Library.Version}", Library.RepositoryUrl))
                .AddBlankInlineField()
                .WithFooter("Use el/help to see bot commands")
                .WithTimestamp(DateTimeOffset.Now)
            );
        }
    }
}
using System;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qmmands;

namespace EmoteLoaf.Disqord
{
    public class EmoteLoafBot : DiscordBot
    {
        public EmoteLoafBot(IOptions<DiscordBotConfiguration> options, ILogger<DiscordBot> logger, 
            IServiceProvider services, DiscordClient client) 
            : base(options, logger, services, client)
        { }

        
    }
}
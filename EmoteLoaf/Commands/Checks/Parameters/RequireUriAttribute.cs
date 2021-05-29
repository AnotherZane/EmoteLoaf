using System;
using System.Threading.Tasks;
using Disqord.Bot;
using Qmmands;

namespace EmoteLoaf.Commands.Checks.Parameters
{
    public sealed class RequireUriAttribute : DiscordParameterCheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(object argument, DiscordCommandContext context)
            =>Uri.IsWellFormedUriString(argument as string, UriKind.Absolute) ? 
                Success() : Failure("The provided argument must be a url.");
    }
}
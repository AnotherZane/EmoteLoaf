using System;
using Disqord;

namespace EmoteLoaf.Extensions
{
    public static class GuildExtensions
    {
        public static int MaxEmoteCount(this IGuild guild)
            => guild.BoostTier switch
            {
                BoostTier.None => 50,
                BoostTier.First => 100,
                BoostTier.Second => 150,
                BoostTier.Third => 250,
                // Thanks again kiel
                _ => throw new ArgumentOutOfRangeException(nameof(guild.BoostTier))
            };
    }
}
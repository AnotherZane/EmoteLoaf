using System;
using Disqord;

namespace EmoteLoaf
{
    public static class Global
    {
        public static readonly Color DefaultEmbedColor = new(0x2F3136);

        public static readonly string BotName = "EmoteLoaf";
        
        public static readonly string BotVersion = "0.1.0";
        
        public static readonly string BotRepo = "https://github.com/AnotherZane/EmoteLoaf";
        
        //public static readonly string UserAgent = $"EmoteLoaf ({BotRepo}, {BotVersion})";

        // Needs to be constant to use as attribute params
        public const string EmoteNameRegex = "^[a-zA-Z0-9_]{2,32}$";

        public static readonly string[] ValidMediaTypes = {"image/png", "image/jpeg", "image/gif", "image/webp", "application/zip"};
        
        public static readonly string[] ValidExtensions = {"png", "jpeg", "jpg", "gif", "webp"};
        
        public static readonly int MaxEmoteSize = 256 * (int)Math.Pow(2, 10);
        
        public static readonly int MaxFileSize = 16 *  (int)Math.Pow(2, 20);
        
        public static Permission InvitePermissions = (Permission.AddReactions
                                                          | Permission.AttachFiles
                                                          | Permission.EmbedLinks
                                                          | Permission.ManageEmojis
                                                          | Permission.ManageMessages
                                                          | Permission.SendMessages
                                                          | Permission.ReadMessageHistory
                                                          | Permission.UseExternalEmojis
                                                          | Permission.ViewAuditLog);
    }
}
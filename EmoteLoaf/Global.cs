using Disqord;

namespace EmoteLoaf
{
    public static class Global
    {
        public static readonly Color DefaultEmbedColor = new(0x2F3136);

        public static string BotName = "EmoteLoaf";
        
        public static string BotVersion = "0.1.0";
        
        public static string BotRepo = "https://github.com/AnotherZane/EmoteLoaf";
        
        public static string UserAgent = $"EmoteLoaf ({BotRepo}, {BotVersion})";

        // Needs to be constant to use as attribute params
        public const string EmoteNameRegex = "^[a-zA-Z0-9_]{2,32}$";
        
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
using Disqord;

namespace EmoteLoaf
{
    public static class Global
    {
        public static readonly Color DefaultEmbedColor = new(0x2F3136);
        
        public static string BotRepo = "https://github.com/AnotherZane/EmoteLoaf";
        
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
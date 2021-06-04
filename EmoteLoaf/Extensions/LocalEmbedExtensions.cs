using Disqord;

namespace EmoteLoaf.Extensions
{
    public static class LocalEmbedExtensions
    {
        public static LocalEmbed WithDefaultColor(this LocalEmbed le)
            => le.WithColor(Global.DefaultEmbedColor);
        
        public static LocalEmbed AddInlineField(this LocalEmbed le, string name, string value)
            => le.AddField(name, value, true);
        
        public static LocalEmbed AddInlineField(this LocalEmbed le, string name, object value)
            => le.AddField(name, value, true);

        public static LocalEmbed AddBlankInlineField(this LocalEmbed le)
            => le.AddBlankField(true);
    }
}
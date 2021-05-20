using Disqord;

namespace EmoteLoaf.Extensions
{
    public static class LocalEmbedBuilderExtensions
    {
        public static LocalEmbedBuilder WithDefaultColor(this LocalEmbedBuilder leb)
            => leb.WithColor(Global.DefaultEmbedColor);
        
        public static LocalEmbedBuilder AddInlineField(this LocalEmbedBuilder leb, string name, string value)
            => leb.AddField(name, value, true);
        
        public static LocalEmbedBuilder AddInlineField(this LocalEmbedBuilder leb, string name, object value)
            => leb.AddField(name, value, true);

        public static LocalEmbedBuilder AddBlankInlineField(this LocalEmbedBuilder leb)
            => leb.AddBlankField(true);
    }
}
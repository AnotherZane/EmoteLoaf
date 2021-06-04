using System.Text.RegularExpressions;
using Disqord;

namespace EmoteLoaf
{
    public static class Utils
    {
        private static readonly Regex NameSanitizationRegex = new Regex("[^a-zA-Z0-9_]", RegexOptions.Compiled);
        
        public static string MarkdownLink(string title, string url)
            => Markdown.Link(title, url);

        public static string MarkdownLink(string title, string url, string tooltip)
            => $"[{title}]({url} \"{tooltip}\")";

        public static string FormatEmoteName(string name)
        {
            NameSanitizationRegex.Replace(name, "");

            if (string.IsNullOrEmpty(name))
            {
                name = "emote";
            }
            else if (name.Length < 2)
            {
                name += "_";
            }
            else if (name.Length > 32)
            {
                name = name.Substring(0, 31);
            }

            return name;
        }
    }
}
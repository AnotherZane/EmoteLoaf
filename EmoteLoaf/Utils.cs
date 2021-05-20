using Disqord;

namespace EmoteLoaf
{
    public static class Utils
    {
        public static string MarkdownLink(string title, string url)
            => Markdown.Link(title, url);

        public static string MarkdownLink(string title, string url, string tooltip)
            => $"[{title}]({url} \"{tooltip}\")";
    }
}
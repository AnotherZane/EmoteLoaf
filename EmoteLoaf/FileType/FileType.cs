namespace EmoteLoaf.FileType
{
    using System.IO;

    public class FileType
    {
        private readonly FileTypeMatcher _FileTypeMatcher;

        public string Name { get; }
        public string Extension { get; }
        
        public string MediaType { get; }

        public static FileType Unknown => new FileType("unknown", string.Empty, "unknown", null);

        public FileType(string name, string extension, string mediaType, FileTypeMatcher matcher)
        {
            Name = name;
            Extension = extension;
            MediaType = mediaType;
            _FileTypeMatcher = matcher;
        }

        public bool Matches(Stream stream)
        {
            return _FileTypeMatcher == null || _FileTypeMatcher.Matches(stream);
        }
        
        public bool Matches(byte[] stream)
        {
            return _FileTypeMatcher == null || _FileTypeMatcher.Matches(stream);
        }
    }
}

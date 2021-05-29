namespace MimeGuesser
{
    using System.IO;

    public class FileType
    {
        private readonly FileTypeMatcher _fileTypeMatcher;

        public string Name { get; }
        public string Extension { get; }
        
        public string Mime { get; }

        public static FileType Unknown => new FileType("unknown", string.Empty, "unknown", null);

        public FileType(string name, string extension, string mime, FileTypeMatcher matcher)
        {
            Name = name;
            Extension = extension;
            Mime = mime;
            _fileTypeMatcher = matcher;
        }

        public bool Matches(Stream stream)
        {
            return _fileTypeMatcher == null || _fileTypeMatcher.Matches(stream);
        }
        
        public bool Matches(byte[] stream)
        {
            return _fileTypeMatcher == null || _fileTypeMatcher.Matches(stream);
        }
    }
}

namespace MimeGuesser
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class FileTypeGuesser
    {
        private readonly IList<FileType> _knownFileTypes;

        public FileTypeGuesser()
        {
            _knownFileTypes = new List<FileType>
            {
                new FileType("Portable Network Graphic", "png", "image/png",
                    new ExactFileTypeMatcher(new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A})),
                new FileType("JPEG", "jpeg", "image/jpeg",
                    new FuzzyFileTypeMatcher(new byte?[] {0xFF, 0xD8, 0xFF, null, null, null, 0x4A, 0x46, 0x49, 0x46, 0x00})),
                new FileType("Graphics Interchange Format 87a", "gif", "image/gif",
                    new ExactFileTypeMatcher(new byte[] {0x47, 0x49, 0x46, 0x38, 0x37, 0x61})),
                new FileType("Graphics Interchange Format 89a", "gif", "image/gif",
                    new ExactFileTypeMatcher(new byte[] {0x47, 0x49, 0x46, 0x38, 0x39, 0x61})),
                new FileType("WebP", ".webp", "image/webp",
                    new FuzzyFileTypeMatcher(new byte?[] {0x52, 0x49, 0x46, 0x46, null, null, null, null, 0x57, 0x45, 0x42, 0x50}))
            };
        }

        public FileTypeGuesser(IList<FileType> knownFileTypes)
        {
            _knownFileTypes = knownFileTypes;
        }
        
        public FileType GuessMimeType(Stream fileContent)
        {
            return GuessMimeTypes(fileContent).FirstOrDefault() ?? FileType.Unknown;
        }

        public IEnumerable<FileType> GuessMimeTypes(Stream stream)
        {
            return _knownFileTypes.Where(fileType => fileType.Matches(stream));
        }
        
        public FileType GuessMimeType(byte[] fileContent)
        {
            return GuessMimeTypes(fileContent).FirstOrDefault() ?? FileType.Unknown;
        }

        public IEnumerable<FileType> GuessMimeTypes(byte[] stream)
        {
            return _knownFileTypes.Where(fileType => fileType.Matches(stream));
        }
    }
}

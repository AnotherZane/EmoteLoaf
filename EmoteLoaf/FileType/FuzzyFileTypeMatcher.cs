namespace EmoteLoaf.FileType
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class FuzzyFileTypeMatcher : FileTypeMatcher
    {
        private readonly byte?[] _bytes;

        public FuzzyFileTypeMatcher(IEnumerable<byte?> bytes)
        {
            _bytes = bytes.ToArray();
        }

        protected override bool MatchesPrivate(Stream stream)
        {
            foreach (var b in _bytes)
            {
                var c = stream.ReadByte();
                if (c == -1 || (b.HasValue && c != b.Value))
                    return false;
            }

            return true;
        }
        
        protected override bool MatchesPrivate(byte[] bytes, long position)
        {
            long i = position;
            
            foreach (var b in _bytes)
            {
                if (i > bytes.LongLength)
                    return false;
                
                var c = bytes[i];
                if (b.HasValue && c != b.Value)
                    return false;

                i++;
            }

            return true;
        }
    }
}

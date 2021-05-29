namespace MimeGuesser
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class ExactFileTypeMatcher : FileTypeMatcher
    {
        private readonly byte[] _bytes;

        public ExactFileTypeMatcher(IEnumerable<byte> bytes)
        {
            _bytes = bytes.ToArray();
        }

        protected override bool MatchesPrivate(Stream stream)
        {
            foreach (var b in _bytes)
            {
                if (stream.ReadByte() != b)
                    return false;
            }
            
            return true;
        }
        
        protected override bool MatchesPrivate(byte[] bytes, long position)
        {
            long i = position;
            
            foreach (var b in _bytes)
            {
                if (bytes[i] != b)
                    return false;

                i++;
            }

            return true;
        }
    }
}

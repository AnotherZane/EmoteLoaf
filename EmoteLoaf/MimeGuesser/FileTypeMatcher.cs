namespace MimeGuesser
{
    using System;
    using System.IO;

    public abstract class FileTypeMatcher
    {
        public bool Matches(Stream stream, bool resetPosition = true)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (!stream.CanRead || (stream.Position != 0 && !stream.CanSeek))
            {
                throw new ArgumentException("File contents must be a readable stream", "stream");
            }
            if (stream.Position != 0 && resetPosition)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            return MatchesPrivate(stream);
        }

        protected abstract bool MatchesPrivate(Stream stream);
        
        public bool Matches(byte[] bytes, long position = 0)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("stream");
            }

            return MatchesPrivate(bytes, position);
        }

        protected abstract bool MatchesPrivate(byte[] bytes, long position);
    }
}

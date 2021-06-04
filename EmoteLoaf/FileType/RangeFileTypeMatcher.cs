using System;

namespace EmoteLoaf.FileType
{
    using System.IO;

    public class RangeFileTypeMatcher : FileTypeMatcher
    {
        private readonly FileTypeMatcher _matcher;

        private readonly int _maximumStartLocation;

        public RangeFileTypeMatcher(FileTypeMatcher matcher, int maximumStartLocation)
        {
            if (matcher is RangeFileTypeMatcher)
                throw new ArgumentException("matcher cannot be of type RangeFileTypeMatcher");
            
            _matcher = matcher;
            _maximumStartLocation = maximumStartLocation;
        }

        protected override bool MatchesPrivate(Stream stream)
        {
            for (var i = 0; i < _maximumStartLocation; i++)
            {
                stream.Position = i;
                
                if (_matcher.Matches(stream, resetPosition: false))
                    return true;
            }

            return false;
        }
        
        protected override bool MatchesPrivate(byte[] bytes, long position)
        {
            for (var i = 0; i < _maximumStartLocation; i++)
            {
                if (_matcher.Matches(bytes, i))
                    return true;
            }

            return false;
        }
    }
}

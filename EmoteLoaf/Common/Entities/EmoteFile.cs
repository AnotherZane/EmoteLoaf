using System.IO;
using Disqord;

namespace EmoteLoaf.Common.Entities
{
    public class EmoteFile
    {
        public MemoryStream Stream { get; }
     
        public string Name { get; }
        
        public string MediaType { get; }

        public EmoteFile(byte[] bytes, string mediaType, string name = "")
        {
            Stream = new MemoryStream(bytes);
            Name = name;
            MediaType = mediaType;
        }
        
        public EmoteFile(MemoryStream stream, string mediaType, string name = "")
        {
            Stream = stream;
            Name = name;
            MediaType = mediaType;
        }
    }
}
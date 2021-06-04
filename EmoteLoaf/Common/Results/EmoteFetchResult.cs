using System.Collections.Generic;
using EmoteLoaf.Common.Entities;

namespace EmoteLoaf.Common.Results
{
    public abstract class EmoteFetchResult
    {
        private EmoteFetchResult() { }

        public abstract bool IsSuccessful { get; }
        
        public sealed class Single : EmoteFetchResult
        {
            public EmoteFile File { get; }
            
            public override bool IsSuccessful => true;

            public Single(EmoteFile file)
            {
                File = file;
            }
        }
        
        public sealed class Multiple : EmoteFetchResult
        {
            public IList<EmoteFile> Files { get; }
            
            public override bool IsSuccessful => true;

            public Multiple(params EmoteFile[] files)
            {
                Files = files;
            }
        }
        
        public sealed class Failed : EmoteFetchResult
        {
            public string Message { get; }

            public override bool IsSuccessful => false;

            public Failed(string message)
            {
                Message = message;
            }
        }
    }

    
}
using System.Collections.Generic;

namespace MediaCleaner.Interfaces
{
    public interface IDatabaseMediaProvider
    {
        ICollection<string> QueryMedia();
    }
}

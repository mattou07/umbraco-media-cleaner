using System.Collections.Generic;
using MediaCleaner.Models;

namespace MediaCleaner.Interfaces
{
    public interface IDiscBasedMediaProvider
    {     
        CompareResult CompareMedia(ICollection<string> filesInDatabase, bool showBadFilesOnly);

        void MoveMedia(IList<string> deletedItems, string sourcePath, string targetPath);
    }
}

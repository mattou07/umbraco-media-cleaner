using System.Collections.Generic;

namespace MediaCleaner.Models
{
    public class CompareResult
    {
        public int TotalFileCount { get; set; }

        public IList<string> DeleteList { get; set; }

        public Statistics Statistics { get; set; }
    }
}

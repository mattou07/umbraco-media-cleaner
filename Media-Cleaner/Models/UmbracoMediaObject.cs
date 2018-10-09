using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media_Cleaner.Models
{
    public class UmbracoMediaObject
    {
        public string _dbMediaPath;

        public UmbracoMediaObject(string dbMediaPath)
        {
            _dbMediaPath = dbMediaPath;
        }
    }
}

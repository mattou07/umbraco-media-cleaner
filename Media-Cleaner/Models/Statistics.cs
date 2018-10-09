using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Media_Cleaner.Models
{
    public class Statistics
    {
        public int i;
        public int inDB;
        public int notInDB;
        public Statistics()
        {
            i = 0;
            inDB = 0;
            notInDB = 0;
        }
    }
}

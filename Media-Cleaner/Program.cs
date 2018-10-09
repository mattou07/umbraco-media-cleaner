using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media_Cleaner.Controllers;
using Media_Cleaner.Models;


namespace Media_Cleaner
{
    internal class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            log.Info("Program start ================");
            bool performMove = ConfigurationManager.AppSettings["PerformMove"] == "true" || ConfigurationManager.AppSettings["PerformMove"] == "True";
            bool showBadFilesOnly = ConfigurationManager.AppSettings["ShowBadFilesOnly"] == "true" || ConfigurationManager.AppSettings["ShowBadFilesOnly"] == "True";
            string mediaLocation = ConfigurationManager.AppSettings["MediaLocation"];
            string connString = ConfigurationManager.AppSettings["ConnectionString"];
            HashSet<string> physicalFiles = FileHandler.ScanMediaOnDisk(mediaLocation);

            HashSet<string> filesInDb = DatabaseHandler.QueryDatabaseMedia(connString);
            //Statistics object to store our statistics across functions
            var statistics = new Statistics();

            List<string> deleteList = FileHandler.CompareMedia(filesInDb, physicalFiles, showBadFilesOnly, statistics);

            if (performMove)
            {
                FileHandler.MoveMedia(deleteList);
            }
            string result =
                $"Stats: Out of {physicalFiles.Count} Physical files, {statistics.inDB} were found in DB, {statistics.notInDB} were found to be orphaned ";
            log.Warn(result);
            log.Info("Program end ================");
            Console.WriteLine("\n Bye!");
        }
    }
}

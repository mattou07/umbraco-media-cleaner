using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using log4net;
using MediaCleaner.Interfaces;
using MediaCleaner.Models;
using MediaCleaner.Services;

namespace MediaCleaner
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            Log.Info("Program start ================");

            bool.TryParse(ConfigurationManager.AppSettings["PerformMove"], out bool performMove);
            bool.TryParse(ConfigurationManager.AppSettings["ShowBadFilesOnly"], out bool showBadFilesOnly);

            string mediaDirectory = ConfigurationManager.AppSettings["MediaLocation"];
            string connString = ConfigurationManager.AppSettings["ConnectionString"];

            IDatabaseMediaProvider databaseMedia = new DatabaseMediaService(connString);
            IDiscBasedMediaProvider discMedia = new DiscBasedMediaService(mediaDirectory);

            ICollection<string> filesInDb = databaseMedia.QueryMedia(); 
            CompareResult result = discMedia.CompareMedia(filesInDb, showBadFilesOnly);

            if (performMove)
            {
                string targetPath = ConfigurationManager.AppSettings["DropOffLocation"];
                string sourcePath = ConfigurationManager.AppSettings["MediaLocation"];
                discMedia.MoveMedia(result.DeleteList, sourcePath, targetPath);
            }

            Statistics statistics = result.Statistics;

            Log.Warn($"Stats: Out of {result.TotalFileCount} Physical files, {statistics.InDb} were found in DB, {statistics.NotInDb} were found to be orphaned ");
            Log.Info("Program end ================");
            Log.Info("\n Bye!");
        }
    }
}
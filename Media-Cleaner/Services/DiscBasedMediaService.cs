using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using MediaCleaner.Interfaces;
using MediaCleaner.Models;

namespace MediaCleaner.Services
{
    public class DiscBasedMediaService : IDiscBasedMediaProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _mediaDirectory;

        public DiscBasedMediaService(string mediaDirectory)
        {
            this._mediaDirectory = mediaDirectory;
        }

        private ICollection<string> ScanMediaOnDisk()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(this._mediaDirectory, "media"));
            
            if (!directoryInfo.Exists)
            {
                Log.Warn(directoryInfo.FullName + " Path not found!");
                throw new DirectoryNotFoundException();
            }

            DirectoryInfo[] mediaFolders = directoryInfo.GetDirectories();

            if (mediaFolders.Length == 0)
            {
                Log.Warn(directoryInfo.FullName + "doesn't contain any media folders");
                throw new FileNotFoundException();
            }

            IList<string> mediaFiles = new List<string>();

            // TODO: What is this doing and why?
            Regex pathCleaningRegex = new Regex(@"^\\*");

            foreach (string file in Directory.EnumerateFiles(directoryInfo.FullName, "*.*", SearchOption.AllDirectories))
            {
                string cleanedPath = pathCleaningRegex.Replace(file.Replace(this._mediaDirectory, ""), "");
                mediaFiles.Add(@"\" + cleanedPath);
            }

            ICollection<string> mediaFileHashSet = new HashSet<string>(mediaFiles);
            return mediaFileHashSet;
        }

        public CompareResult CompareMedia(ICollection<string> filesInDb, bool showBadFilesOnly)
        {
            ICollection<string> filesOnDisc = this.ScanMediaOnDisk();
            int fileCount = filesOnDisc.Count;

            Statistics statistics = new Statistics();

            IList<string> deleteList = new List<string>();
            
            foreach (string mediaItem in filesOnDisc)
            {
                statistics.CurrentFileNumber++;

                if (filesInDb.Contains(mediaItem))
                {
                    statistics.InDb++;
                    if (!showBadFilesOnly) { Log.Info("Found in DB " + mediaItem); }
                }
                else
                {
                    Log.Warn("!Not in DB queued for moving " + mediaItem);
                    deleteList.Add(mediaItem);
                    statistics.NotInDb++;
                }

                Log.Info($"\r Processing media items {statistics.CurrentFileNumber} out of {fileCount}. {statistics.InDb} found in DB, {statistics.NotInDb} orphaned physical files");
            }
           
            if (statistics.NotInDb == filesOnDisc.Count)
            {
                //Failsafe just in case all media is marked as not being in Umbraco DB
                Log.Warn("All Media Items were identified as not being in Umbraco Media, you may have supplied a bad path. ABORTING!");
                throw new InvalidOperationException();
            }

            return new CompareResult { DeleteList = deleteList, Statistics = statistics, TotalFileCount = fileCount};          
        }
        
        public void MoveMedia(IList<string> deletedItems, string sourcePath, string targetPath)
        {

            string depositFolder = Path.Combine(targetPath, "badMedia-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

            if (!Directory.Exists(depositFolder))
            {
                Directory.CreateDirectory(depositFolder);
            }

            //Remove any slashes at the beginning and end of the filename
            //Path.Combine doesn't work if there are slashes on the beginning and end
            Regex removeBackSlashRegex = new Regex(@"\\*$");
            Regex removeFrontSlashRegex = new Regex(@"^\\");
            sourcePath = removeBackSlashRegex.Replace(sourcePath, "");
            depositFolder = removeBackSlashRegex.Replace(depositFolder, "");

            int fileCounter = 0;
            foreach (var mediaItem in deletedItems)
            {
                fileCounter++;
                Log.Info($"\r Moving media items {fileCounter} out of {deletedItems.Count}");
                string source = Path.Combine(sourcePath, removeFrontSlashRegex.Replace(mediaItem, ""));
                string destination = Path.Combine(depositFolder, removeFrontSlashRegex.Replace(mediaItem, ""));

                DirectoryInfo file = new DirectoryInfo(Path.GetDirectoryName(destination));

                // If the directory already exists, this method does nothing.
                file.Create(); 

                File.Move(source, destination);
                Log.Info("Moving " + mediaItem);
            }          
        }
    }
}
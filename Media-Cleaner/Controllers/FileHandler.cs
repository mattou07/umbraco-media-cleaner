using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using Media_Cleaner.Models;

namespace Media_Cleaner.Controllers
{
    public static class FileHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static HashSet<string> ScanMediaOnDisk(string mediaLocation)
        {
            var d = new DirectoryInfo(mediaLocation + @"\media");
            
            if (!d.Exists)
            {
                log.Warn(mediaLocation + @"\media" + " Path not found!");
                throw new DirectoryNotFoundException();
            }

            var mediaFolders = d.GetDirectories();
            if (mediaFolders.Length == 0)
            {
                log.Warn("Specified folder doesn't contain any media folders");
                throw new FileNotFoundException();
            }
            var mediaObjectList = new List<string>();
            foreach (var folder in mediaFolders)
            {
                var files = folder.GetFiles();
                foreach (var file in files)
                {
                    //if \\ is at the beginning of the path this can cause the file to be flagged for deletion
                    Regex regex = new Regex(@"^\\*");
                    var cleanedPath = regex.Replace(file.FullName.Replace(mediaLocation, ""),"");
                    
                    mediaObjectList.Add(@"\"+ cleanedPath);
                }
            }

            var test = new HashSet<string>(mediaObjectList);
            return test;
        }

        public static List<string> CompareMedia(HashSet<string> filesInDb, HashSet<string> physicalFiles, bool showBadFilesOnly, Statistics statistics)
        {
            List<string> deleteList = new List<string>();
            
            foreach (var mediaItem in physicalFiles)
            {
                statistics.i++;

                if (filesInDb.Contains(mediaItem))
                {
                    statistics.inDB++;
                    if (!showBadFilesOnly) { log.Info("Found in DB " + mediaItem); }
                }
                else
                {
                    log.Warn("!Not in DB queued for moving " + mediaItem);
                    deleteList.Add(mediaItem);
                    statistics.notInDB++;
                }
                Console.Write("\r Processing media items {0} out of {1}. {2} found in DB, {3} orphaned physical files", statistics.i, physicalFiles.Count, statistics.inDB, statistics.notInDB);
            }
            Console.WriteLine("\n");
            if (statistics.notInDB == physicalFiles.Count)
            {
                //Failsafe just in case all media is marked as not being in Umbraco DB
                log.Warn("All Media Items were identified as not being in Umbraco Media, you may have supplied a bad path. ABORTING!");
                throw new InvalidOperationException();
            }

            return deleteList;
        }

       
        public static void MoveMedia(List<string> deletedItems)
        {
            Regex regex = new Regex(@"\\+$");
            string targetPath = ConfigurationManager.AppSettings["DropOffLocation"] ;
            if (!regex.IsMatch(targetPath))
            {
                targetPath += "\\";
            }
            string sourcePath = ConfigurationManager.AppSettings["MediaLocation"] ;
            string depositFolder = targetPath + "badMedia-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            if (!System.IO.Directory.Exists(depositFolder))
            {
                System.IO.Directory.CreateDirectory(depositFolder);
            }
            //Remove any slashes at the end of the filename
            regex = new Regex(@"\\*$");
            targetPath = regex.Replace(targetPath, "");
            sourcePath = regex.Replace(sourcePath, "");
            int i = 0;
            foreach (var mediaItem in deletedItems)
            {
                i++;
                Console.Write("\r Moving media items {0} out of {1}",i, deletedItems.Count);
                string source = sourcePath + mediaItem;
                string destination = depositFolder + mediaItem;
                System.IO.DirectoryInfo file = new System.IO.DirectoryInfo(Path.GetDirectoryName(destination));
                file.Create(); // If the directory already exists, this method does nothing.
                System.IO.File.Move(source, destination);
                log.Info("Moving " + mediaItem);
            }
            
        }
    }
}

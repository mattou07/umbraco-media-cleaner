using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.IO;
using MediaCleaner.Interfaces;
using MediaCleaner.Models;

namespace MediaCleaner.Services.Tests
{
    [TestClass]
    public class FileHandlerTest
    {
        [TestMethod]
        public void BadMediaLocation()
        {

            string badMediaLocation = ConfigurationManager.AppSettings["NonExistantFileDirectory"];
            IDiscBasedMediaProvider discMedia = new DiscBasedMediaService(badMediaLocation);

            try
            {
                CompareResult test = discMedia.CompareMedia(null, false);
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
            Assert.Fail("Bad directory supplied did not trigger DirectoryNotFoundException");
        }

        [TestMethod]
        public void GoodMediaLocationWithSlash()
        {
            try
            {
                string goodMediaLocation = ConfigurationManager.AppSettings["MediaLocationWithEndSlash"];
                IDiscBasedMediaProvider discMedia = new DiscBasedMediaService(goodMediaLocation);

                CompareResult test = discMedia.CompareMedia(new HashSet<string>(), false);

                if (test.TotalFileCount == 0)
                {
                    Assert.Fail("Double check Media Location is valid, HashSet is empty");
                }

                return;
            }
            catch (DirectoryNotFoundException)
            {
                Assert.Fail("Double check Media Location is valid");
            }

            
            
        }
        [TestMethod]
        public void GoodMediaLocationWithoutSlash()
        {
            try
            {
                string goodMediaLocation = ConfigurationManager.AppSettings["MediaLocationWithoutEndSlash"];
                string goodDBConnectionString = ConfigurationManager.AppSettings["GoodConnectionString"];


                IDiscBasedMediaProvider discMedia = new DiscBasedMediaService(goodMediaLocation);
                IDatabaseMediaProvider databaseMedia = new DatabaseMediaService(goodDBConnectionString);

                CompareResult test = discMedia.CompareMedia(null, false);

                if (test.TotalFileCount == 0)
                {
                    Assert.Fail("Double check Media Location is valid, HashSet is empty");
                }

                ICollection<string> dbTest = databaseMedia.QueryMedia();
                Assert.IsTrue(dbTest.Count > 0);
            }
            catch (DirectoryNotFoundException)
            {
                Assert.Fail("Double check Media Location is valid");
            }

        }

        [TestMethod]
        public void NoMediaInDirectory()
        {
            string badMediaLocation = ConfigurationManager.AppSettings["NonExistantFileDirectory"];
            IDiscBasedMediaProvider discMedia = new DiscBasedMediaService(badMediaLocation);

            try
            {

                CompareResult test = discMedia.CompareMedia(null, false);

            }
            catch (FileNotFoundException)
            {
                return;
            }
            Assert.Fail("Bad directory supplied did not trigger DirectoryNotFoundException");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanMediaOnDiskMediapathNull()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanMediaOnDiskMediapathNullTryCathc()
        {
            Assert.Inconclusive();
            try
            {

            }
            catch (ArgumentNullException)
            {
                //Test passed
                return;
            }
            Assert.Fail("Useful message used here");
        }
    }
}

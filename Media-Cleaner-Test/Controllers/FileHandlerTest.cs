using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Media_Cleaner.Controllers;
using System.Configuration;
using System.IO;

namespace Media_Cleaner.Controllers.Tests
{
    [TestClass]
    public class FileHandlerTest
    {
        [TestMethod]
        public void BadMediaLocation()
        {
            try
            {
                string badMediaLocation = ConfigurationManager.AppSettings["NonExistantFileDirectory"];
                HashSet<string> test = FileHandler.ScanMediaOnDisk(badMediaLocation);
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
                string GoodMediaLocation = ConfigurationManager.AppSettings["MediaLocationWithEndSlash"];
                HashSet<string> test = FileHandler.ScanMediaOnDisk(GoodMediaLocation);
                if (test.Count == 0)
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
                string GoodMediaLocation = ConfigurationManager.AppSettings["MediaLocationWithoutEndSlash"];
                string GoodDBConnectionString = ConfigurationManager.AppSettings["GoodConnectionString"];
                
                HashSet<string> test = FileHandler.ScanMediaOnDisk(GoodMediaLocation);
                if (test.Count == 0)
                {
                    Assert.Fail("Double check Media Location is valid, HashSet is empty");
                }

                HashSet<string> dbTest = DatabaseHandler.QueryDatabaseMedia(GoodDBConnectionString);



                return;
            }
            catch (DirectoryNotFoundException)
            {
                Assert.Fail("Double check Media Location is valid");
            }



        }

        [TestMethod]
        public void NoMediaInDirectory()
        {
            try
            {
                string badMediaLocation = ConfigurationManager.AppSettings["MediaLocationWithoutMedia"];
                HashSet<string> test = FileHandler.ScanMediaOnDisk(badMediaLocation);
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

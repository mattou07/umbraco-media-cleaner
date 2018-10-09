using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media_Cleaner.Models;

namespace Media_Cleaner.Controllers
{
    public class DatabaseHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static HashSet<string> QueryDatabaseMedia(string connString)
        {
            const string queryString = "SELECT distinct dataNvarchar FROM [dbo].[cmsPropertyData] where dataNvarChar like '/media/%'";
            
            var umbracoMediaObjectList = new List<string>();
            try
            {
                using (var connection = new SqlConnection(connString))
                {
                    var command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            umbracoMediaObjectList.Add(reader["dataNvarchar"].ToString().Replace("/", "\\"));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                log.Info(ex);
                log.Warn("Issue with connecting to Database, please check your connection string.");
                Environment.Exit(0);
            }
            
            var umbracoMediaObjects = new HashSet<string>(umbracoMediaObjectList);
            return umbracoMediaObjects;
        } 
    }
}

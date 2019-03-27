using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using log4net;
using MediaCleaner.Interfaces;

namespace MediaCleaner.Services
{
    public class DatabaseMediaService : IDatabaseMediaProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _connectionString;

        public DatabaseMediaService(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public ICollection<string> QueryMedia()
        {
            // TODO: there is probably a better way to get all media...
            // Also this won't work for image croppers etc...

            const string query = "SELECT distinct dataNvarchar FROM [dbo].[cmsPropertyData] where dataNvarChar like '/media/%'";           
            IList<string> umbracoMediaList = new List<string>();

            try
            {
                using (var connection = new SqlConnection(this._connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    try
                    {
                        while (reader.Read())
                        {
                            string mediaItem = reader["dataNvarchar"].ToString();
                            mediaItem = mediaItem.Replace("/", "\\");

                            umbracoMediaList.Add(mediaItem);
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
                Log.Fatal(ex);
                Log.Warn("Issue connecting to Database, please check your connection string.");

                throw ex;
            }
            
            return new HashSet<string>(umbracoMediaList);           
        }
    }
}
using MongoDB.Driver;

namespace LCMVC.DatabaseHelper
{
    public class MongoDBHelper
    {
        private static IMongoDatabase _database;
        private static bool connected = false;
        public static String ErrorMessage = "";

        public static IMongoDatabase Database
        {
            get
            {
                if (connected == false)
                {
                    ConnectionDatabase();
                }
                return _database;
            }
        }

        private static void ConnectionDatabase()
        {
            try
            {
                var configuration = WebApplication.CreateBuilder().Configuration;
                var connectionString = configuration.GetConnectionString("MongoDB");
                var databaseName = configuration.GetValue<string>("DatabaseName");
                var client = new MongoClient(connectionString);

                _database = client.GetDatabase(databaseName);
                ErrorMessage = "";
                connected = true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                connected = false;
            }
        }
    }
}

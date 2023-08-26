using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using OfficeOpenXml;

namespace LCMVC.DatabaseHelper
{
    public class UserInfo
    {
        public static String CollectionName = "user";

        private static IMongoCollection<UserInfo> _collation;
        private static bool connected = false;

        public static IMongoCollection<UserInfo> DBCollation
        {
            get
            {
                if (connected == false)
                {
                    _collation = MongoDBHelper.Database.GetCollection<UserInfo>(CollectionName);
                }
                return _collation;
            }
        }

        public static UserInfo? VerifyUser(string username, string password)
        {
            return UserInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Username == username && t.Password == password);
        }

        public static UserInfo? GetUser(string username)
        {
            return UserInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Username == username);
        }

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json.Serialization;

namespace LCAPI.Models
{
    /// <summary>
    /// user basic info, see C# Model or JS class
    /// </summary>
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

        public static UserInfo? GetUserById(string id)
        {
            return UserInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// mongoDB ID
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        /// <summary>
        /// "user" for search and download, "admin" for upload
        /// </summary>
        [BsonElement("type")]
        public string Type { get; set; }
    }
}

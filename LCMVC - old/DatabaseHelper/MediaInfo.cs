using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using OfficeOpenXml;
using System.Text.Json.Serialization;

namespace LCMVC.DatabaseHelper
{
    public class MediaInfo
    {
        public static String CollectionName = "media";

        private static IMongoCollection<MediaInfo> _collation;
        private static bool connected = false;

        public static IMongoCollection<MediaInfo> DBCollation
        {
            get
            {
                if (connected == false)
                {
                    _collation = MongoDBHelper.Database.GetCollection<MediaInfo>(CollectionName);
                }
                return _collation;
            }
        }

        //public static MediaInfo? GetUser(string username)
        //{
        //    return MediaInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Username == username);
        //}

        //public static List<MediaInfo> GetMediasByTitle(string title)
        //{
        //    return MediaInfo.DBCollation.AsQueryable().Where(t => t.title.Contains(title)).ToList();
        //}

        // 不一定有
        [BsonId]
        public ObjectId Id { get; set; }

        public string mediatype { get; set; }
        public string mediatitle { get; set; }  
        public string mediatitle_zh { get; set; }
        public string mediakeyword { get; set; }
        public string mediaarea { get; set; }
        public Int64 mediaplaycount { get; set; }
        public Double mediaaverstar { get; set; }

        // 服务器更新
        public String mediauploaddate { get; set; }
        // 服务器更新
        public ObjectId mediauploaduserid { get; set; }
        public string mediaextension { get; set; }
        public string mediainfo { get; set; }
        public string mediapath { get; set; }
        public string mediapublishdate { get; set; }
        public string mediaquality { get; set; }
        public string mediatime { get; set; }
    }
}

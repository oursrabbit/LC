using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace LCAPI.Models
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

        public UserInfo()
        {
            Id = ObjectId.Empty;
            username = "";
            password = "";
            type = "";
        }

        public UserInfo(UserInfoJSON json)
        {
            var newId = ObjectId.Empty;
            Id = ObjectId.TryParse(json.id ?? "", out newId) ? newId : ObjectId.Empty;

            username = json.username ?? "";
            password = json.password ?? "";
            type = json.type ?? "";
        }

        public UserInfoJSON ToUserInfoJSON()
        {
            return new UserInfoJSON(this);
        }

        [BsonId]
        public ObjectId Id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string type { get; set; }

        public static UserInfo? VerifyUser(string username, string password)
        {
            return UserInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.username == username && t.password == password);
        }

        public static UserInfo? GetUser(string username)
        {
            return UserInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.username == username);
        }

        public static UserInfo? GetUserById(string id)
        {
            return UserInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id.ToString() == id);
        }
    }

    /// <summary>
    /// 用户信息类
    /// </summary>
    public class UserInfoJSON
    {
        /// <summary>
        /// MongoDB 内置的主键，一串随机字符
        /// 创建新的实例时，可以不填，由MongoDB自动生成
        /// 查找、更新、删除时，可以使用该列进行数据定位
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 字符串，表示用户类型
        /// "user"：普通用户
        /// "admin"：管理员
        /// </summary>

        public string type { get; set; }

        /// <summary>
        /// 用户名，任意字符串，英文或数字
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 密码，任意字符串
        /// </summary>
        public string password { get; set; }

        public UserInfoJSON()
        {
            id = "";
            type = "";
            username = "";
            password = "";
        }

        public UserInfoJSON(UserInfo user)
        {
            id = user.Id.ToString();
            username = user.username;
            password = user.password;
            type = user.type;
        }
    }
}

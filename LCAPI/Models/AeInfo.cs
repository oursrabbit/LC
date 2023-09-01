using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json.Serialization;
using System.Globalization;
using OfficeOpenXml;
using MongoDB.Driver.Linq;
using System.Net.Quic;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using System.Linq;
using System.Net;

namespace LCAPI.Models
{
    [BsonIgnoreExtraElements]
    public class AeInfo
    {
        public static String CollectionName = "ae";

        private static IMongoCollection<AeInfo> _collation;
        private static bool connected = false;

        public static IMongoCollection<AeInfo> DBCollation
        {
            get
            {
                if (connected == false)
                {
                    _collation = MongoDBHelper.Database.GetCollection<AeInfo>(CollectionName);
                }
                return _collation;
            }
        }

        [BsonId]
        public ObjectId Id { get; set; }
        public string resource_type { get; set; } // 资源类型，固定 AE资源
        public List<String> resource_lang { get; set; } // 语种，该类型数据，未使用此列
        public List<String> resource_keyword { get; set; } // 关键字
        public List<String> resource_tag { get; set; }// 标签（模型类型）

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime resource_publish_date { get; set; } // 资源产生时间，Local UTC+8
        public string resource_description { get; set; } // 资源描述
        public string resource_source_uri { get; set; } // 资源来源
        public string resource_file_name { get; set; }// 原始文件名
        public string resource_file_name_zh { get; set; }// 中文文件名
        public string resource_file_extension { get; set; } // 资源文件后缀，固定.mp4
        public string resource_file_size_string { get; set; }// 文件大小，MB

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime resource_upload_date { get; set; }// 资源上传时间，Local UTC+8
        public ObjectId resource_upload_user { get; set; } // 资源上传人
        public string resource_upload_username { get; set; } // 资源上传人

        public double resource_score { get; set; } // 资源评分，最低0分，最高5分

        public Int64 resource_download_count { get; set; }  // 下载次数

        public Int64 resource_view_count { get; set; } // 浏览次数

        public string ae_clarity { get; set; } // 清晰度，任意字符串
        public string ae_download_url { get; set; } // 下载URL
        public Int64 ae_duration { get; set; } // 总秒数
        public List<string> ae_plugins { get; set; } // 使用的插件名称列表
        public string ae_title { get; set; } // 标题
        public string ae_view { get; set; } // 预览URL
        public string ae_version { get; set; } // 版本号

        public AeInfo()
        {
            Id = ObjectId.Empty;
            resource_type = "AE资源";
            resource_lang = new List<string>();
            resource_keyword = new List<string>();
            resource_tag = new List<string>();
            resource_publish_date = DateTime.Now; // UTC+8
            resource_description = "";
            resource_source_uri = "";
            resource_file_name = "";
            resource_file_name_zh = "";
            resource_file_extension = "";
            resource_file_size_string = "";
            resource_upload_date = DateTime.Now; // UTC+8
            resource_upload_user = ObjectId.Empty;
            resource_upload_username = "";
            resource_score = 0;
            resource_download_count = 0;
            resource_view_count = 0;

            ae_clarity = "";
            ae_download_url = "";
            ae_duration = 0;
            ae_plugins = new();
            ae_title = "";
            ae_view = "";
            ae_version = "";

        }

        public AeInfo(AeInfoJSON json)
        {
            var newId = ObjectId.Empty;
            Id = ObjectId.TryParse(json.id ?? "", out newId) ? newId : ObjectId.Empty;

            resource_type = json.resource_type;
            resource_lang = json.resource_lang;
            resource_keyword = json.resource_keyword;
            resource_tag = json.resource_tag;

            var pdate = DateTimeOffset.UtcNow;
            resource_publish_date = DateTimeOffset.TryParse(json.resource_publish_date, out pdate) ? pdate.LocalDateTime : DateTime.Now;

            resource_description = json.resource_description;
            resource_source_uri = json.resource_source_uri;
            resource_file_name = json.resource_file_name;
            resource_file_name_zh = json.resource_file_name_zh;
            resource_file_extension = json.resource_file_extension;
            resource_file_size_string = json.resource_file_size_string;

            var udate = DateTimeOffset.UtcNow;
            resource_upload_date = DateTimeOffset.TryParse(json.resource_upload_date, out udate) ? udate.LocalDateTime : DateTime.Now;

            var uId = ObjectId.Empty;
            resource_upload_user = ObjectId.TryParse(json.resource_upload_user ?? "", out uId) ? uId : ObjectId.Empty;

            resource_upload_username = json.resource_upload_username;
            resource_score = json.resource_score;
            resource_download_count = json.resource_download_count;
            resource_view_count = json.resource_view_count;

            ae_clarity = json.ae_clarity;
            ae_download_url = json.ae_download_url;
            ae_duration = json.ae_duration;
            ae_plugins = json.ae_plugins;
            ae_title = json.ae_title;
            ae_view = json.ae_url;
            ae_version = json.ae_version;
        }

        public AeInfoJSON ToAeInfoJSON()
        {
            return new AeInfoJSON(this);
        }

        public static List<AeInfo> FullTextFind(List<string> keywords, int page, int page_size, string order, out int total_count)
        {
            var query = AeInfo.DBCollation.AsQueryable();

            if (keywords == null || keywords.Count == 0)
            {
                // All
            }
            else
            {
                query = query.Where(t => keywords.All(kw => t.resource_description.Contains(kw))
                                    || keywords.All(kw => t.resource_file_name.Contains(kw))
                                    || keywords.All(kw => t.resource_file_name_zh.Contains(kw))
                                    || keywords.All(kw => t.resource_keyword.Contains(kw))
                                    || keywords.All(kw => t.resource_tag.Contains(kw))
                                    || keywords.All(kw => t.ae_title.Contains(kw))
                                    || keywords.All(kw => t.ae_plugins.Contains(kw)));
            }

            if (order == "asc")
            {
                query = query.OrderBy(t => t.resource_publish_date);
            }
            else
            {
                query = query.OrderByDescending(t => t.resource_publish_date);
            }

            total_count = query.Count();
            query = query.Skip((page - 1) * page_size).Take(page_size);

            return query.ToList();
        }

        public static List<AeInfo> Search(SearchAeJSON search, int page, int page_size, string order, out int total)
        {
            // 视频发布时间范围，如果转换失败，默认起始结束时间都是当前
            var startPublishDate = DateTime.Now;
            var endPublishDate = DateTime.Now;
            var startPublishDateUTC = DateTimeOffset.Now;
            var endPublishDateUTC = DateTimeOffset.Now;
            var startPublishDateConvert = DateTimeOffset.TryParse(search.resource_publish_date_start, out startPublishDateUTC);
            var endPublishDateConvert = DateTimeOffset.TryParse(search.resource_publish_date_end, out endPublishDateUTC);

            startPublishDate = startPublishDateUTC.LocalDateTime;
            endPublishDate = endPublishDateUTC.LocalDateTime;

            var query = AeInfo.DBCollation.AsQueryable();

            if (search.resource_keyword.Count > 0)
            {
                query = query.Where(t => search.resource_keyword.All(kw => t.resource_keyword.Contains(kw)));
            }

            if (search.resource_tag.Count > 0)
            {
                query = query.Where(t => search.resource_tag.All(a => t.resource_tag.Contains(a)));
            }

            if (search.resource_description.Count > 0)
            {
                query = query.Where(t => search.resource_description.All(d => t.resource_description.Contains(d)));
            }

            if (search.ae_title.Count > 0)
            {
                query = query.Where(t => search.ae_title.All(d => t.ae_title.Contains(d)));
            }

            if (search.ae_plugins.Count > 0)
            {
                query = query.Where(t => search.ae_plugins.All(a => t.ae_plugins.Contains(a)));
            }

            if (search.resource_file_name.Count > 0)
            {
                query = query.Where(t => search.resource_file_name.All(f => t.resource_file_name.Contains(f) || t.resource_file_name_zh.Contains(f)));
            }

            if (startPublishDateConvert == true && endPublishDateConvert == true)
            {
                query = query.Where(t =>
                    t.resource_publish_date >= startPublishDate
                    && t.resource_publish_date <= endPublishDate);
            }

            if (order == "asc")
            {
                query = query.OrderBy(t => t.resource_publish_date);
            }
            else
            {
                query = query.OrderByDescending(t => t.resource_publish_date);
            }

            total = query.Count();
            query = query.Skip((page - 1) * page_size).Take(page_size);

            return query.ToList();
        }

        public static AeInfo? UpdateScore(string id, double value)
        {
            var filter = Builders<AeInfo>.Filter
                .Eq(t => t.Id, new ObjectId(id));
            var update = Builders<AeInfo>.Update
                .Set(t => t.resource_score, value);
            var res = AeInfo.DBCollation.UpdateOne(filter, update);
            return AeInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == new ObjectId(id));
        }

        public static AeInfo? UpdateDownloadCount(string id)
        {
            var old = AeInfo.DBCollation.AsQueryable().Where(t => t.Id == new ObjectId(id)).FirstOrDefault();
            var oldValue = old == null ? 0 : old.resource_download_count;

            var filter = Builders<AeInfo>.Filter
                .Eq(t => t.Id, new ObjectId(id));
            var update = Builders<AeInfo>.Update
                .Set(t => t.resource_download_count, oldValue + 1);
            var res = AeInfo.DBCollation.UpdateOne(filter, update);
            return AeInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == new ObjectId(id));
        }

        public static AeInfo? UpdateViewCount(string id)
        {
            var old = AeInfo.DBCollation.AsQueryable().Where(t => t.Id == new ObjectId(id)).FirstOrDefault();

            var oldValue = old == null ? 0 : old.resource_view_count;

            var filter = Builders<AeInfo>.Filter.Eq(t => t.Id, new ObjectId(id));
            var update = Builders<AeInfo>.Update.Set(t => t.resource_view_count, oldValue + 1);
            var res = AeInfo.DBCollation.UpdateOne(filter, update);
            return AeInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == new ObjectId(id));
        }

        public static AeInfo? Insert(AeInfo ae)
        {
            var filter = Builders<AeInfo>.Filter.Eq(t => t.Id, ae.Id);

            if (filter != null)
                AeInfo.DBCollation.DeleteOne(filter);

            AeInfo.DBCollation.InsertOne(ae);
            return AeInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == ae.Id);
        }

        public static AeInfo? Update(AeInfo ae)
        {
            var filter = Builders<AeInfo>.Filter.Eq(t => t.Id, ae.Id);

            if (filter != null)
                AeInfo.DBCollation.DeleteOne(filter);

            AeInfo.DBCollation.InsertOne(ae);
            return AeInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == ae.Id);
        }

        public static void Delete(string id)
        {
            var filter = Builders<AeInfo>.Filter.Eq(t => t.Id, new ObjectId(id));
            var res = AeInfo.DBCollation.DeleteOne(filter);
        }

        public static AeInfo? GetAeInfoById(string id)
        {
            return AeInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == new ObjectId(id));
        }

        public static void UpdateDBFromExcel(string filePath, UserInfo user, bool ignoreError, out string errorString, out List<AeInfo> newAes, out List<int> newErrorAesRowIndex)
        {
            errorString = "";
            newAes = new List<AeInfo>();
            newErrorAesRowIndex = new List<int>();

            try
            {
                FileInfo file = new FileInfo(filePath);
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;
                    for (int rowindex = 3; rowindex <= rowCount; rowindex++)
                    {
                        try
                        {
                            var aeInfo = new AeInfo();
                            aeInfo.Id = ObjectId.Empty;
                            aeInfo.resource_type = "AE资源";

                            aeInfo.resource_lang = (worksheet.Cells[rowindex, 1].Value?.ToString() ?? "").Split(' ').ToList();

                            aeInfo.resource_keyword = (worksheet.Cells[rowindex, 2].Value?.ToString() ?? "").Split(' ').ToList();

                            aeInfo.resource_tag = (worksheet.Cells[rowindex, 3].Value?.ToString() ?? "").Split(' ').ToList();

                            var publishDate = DateTime.Now;
                            var publishDateRes = DateTime.TryParse(worksheet.Cells[rowindex, 4].Value?.ToString() ?? "", out publishDate);
                            if (publishDateRes == false && ignoreError == true)
                            {
                                errorString += $"row_{rowindex}_warring: resource_publish_date format error\r\n\r\n";
                            }
                            else if (publishDateRes == false && ignoreError == false)
                            {
                                throw new Exception("resource_publish_date format error");
                            }
                            aeInfo.resource_publish_date = publishDate;

                            aeInfo.resource_description = worksheet.Cells[rowindex, 5].Value?.ToString() ?? "";
                            aeInfo.resource_source_uri = worksheet.Cells[rowindex, 6].Value?.ToString() ?? "";
                            aeInfo.resource_file_name = worksheet.Cells[rowindex, 7].Value?.ToString() ?? "";
                            aeInfo.resource_file_name_zh = aeInfo.resource_file_name;
                            aeInfo.resource_file_extension = worksheet.Cells[rowindex, 8].Value?.ToString() ?? "";
                            aeInfo.resource_file_size_string = worksheet.Cells[rowindex, 9].Value?.ToString() ?? ""; ;

                            aeInfo.resource_upload_date = DateTime.Now;
                            aeInfo.resource_upload_user = user.Id;
                            aeInfo.resource_upload_username = user.username;
                            aeInfo.resource_score = 0;
                            aeInfo.resource_download_count = 0;
                            aeInfo.resource_view_count = 0;

                            aeInfo.ae_version = worksheet.Cells[rowindex, 10].Value?.ToString() ?? "";

                            var durationRes = true;
                            var duration = worksheet.Cells[rowindex, 11].Value?.ToString() ?? "0:0:0";
                            if (duration.Split(':').Length != 3)
                            {
                                duration = "0:0:0";
                                durationRes = false;
                            }
                            var hour = 0;
                            durationRes = int.TryParse(duration.Split(':')[0], out hour) == true ? durationRes : false;
                            var minute = 0;
                            durationRes = int.TryParse(duration.Split(":")[1], out minute) == true ? durationRes : false;
                            var second = 0;
                            durationRes = int.TryParse(duration.Split(":")[2], out second) == true ? durationRes : false;
                            aeInfo.ae_duration = hour * 3600 + minute * 60 + second;
                            if (durationRes == false && ignoreError == true)
                            {
                                errorString += $"row_{rowindex}_warring: ae_duration format error\r\n\r\n";
                            }
                            else if (durationRes == false && ignoreError == false)
                            {
                                throw new Exception("ae_duration format error");
                            }

                            aeInfo.ae_clarity = worksheet.Cells[rowindex, 12].Value?.ToString() ?? "";
                            aeInfo.ae_download_url = "";
                            aeInfo.ae_plugins = (worksheet.Cells[rowindex, 13].Value?.ToString() ?? "").Split(' ').ToList();
                            aeInfo.ae_title = worksheet.Cells[rowindex, 14].Value?.ToString() ?? "";
                            aeInfo.ae_view = "";

                            AeInfo.DBCollation.InsertOne(aeInfo);
                            newAes.Add(aeInfo);
                        }
                        catch (Exception rowEx)
                        {
                            errorString += $"row_{rowindex}_error: {rowEx.Message}\r\n\r\n";
                            newErrorAesRowIndex.Add(rowindex);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorString += e.Message + "\r\n\r\n";
            }
        }
    }

    /// <summary>
    /// 视频素材数据库
    /// </summary>
    public class AeInfoJSON
    {
        /// <summary>
        /// MongoDB 内置的主键，一串随机字符
        /// 创建新的实例时，可以不填，由MongoDB自动生成
        /// 查找、更新、删除时，可以使用该列进行数据定位
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 固定值：”三维模型“
        /// </summary>
        public string resource_type { get; set; } // 资源类型  

        /// <summary>
        /// 该类型数据，未使用此列
        /// </summary>
        public List<String> resource_lang { get; set; } // 该类型数据，未使用此列

        /// <summary>
        /// 关键词信息
        /// </summary>
        public List<String> resource_keyword { get; set; } // 关键字

        /// <summary>
        /// 模型类型
        /// </summary>
        public List<String> resource_tag { get; set; }// 标签（模型类型）

        /// <summary>
        /// 该资源在网络中被发布的时间，格式为ISO UTC Date
        /// 
        /// 2023-07-19T16:00:00.000+00:00
        /// 
        /// 2023-07-19T16:00:00.000Z
        /// 
        /// 以上两个例子为UTC+0，显示时需要转换
        /// 
        /// 数据库中保存的都是UTC+0
        /// </summary>
        public string resource_publish_date { get; set; } // 资源产生时间

        /// <summary>
        /// 任意长度字符串，一段文字
        /// </summary>
        public string resource_description { get; set; } // 资源描述

        /// <summary>
        /// 资源来源，任意字符串
        /// </summary>
        public string resource_source_uri { get; set; } // 资源来源

        /// <summary>
        /// 原始资源名，任意字符串，同中文文件名
        /// </summary>
        public string resource_file_name { get; set; }// 原始文件名

        /// <summary>
        /// 中文资源名称，任意字符串，同原始文件名
        /// </summary>
        public string resource_file_name_zh { get; set; }// 中文文件名


        /// <summary>
        /// 任意字符串
        /// </summary>
        public string resource_file_extension { get; set; } // 资源文件后缀

        /// <summary>
        /// 任意字符串，请标明单位。例：1KB，2.5MB，5.9GB，7TB，10PB
        /// </summary>
        public string resource_file_size_string { get; set; }// 文件大小

        /// <summary>
        /// 该资源被上传至本系统中的时间
        /// 
        /// 2023-07-19T16:00:00.000+00:00
        /// 
        /// 2023-07-19T16:00:00.000Z
        /// 
        /// 以上两个例子为UTC+0，显示时需要转换
        /// 
        /// 数据库中保存的都是UTC+0
        /// </summary>
        public string resource_upload_date { get; set; }// 资源上传时间

        /// <summary>
        /// 上传用户的MongoDB UserInfo Id字符串
        /// </summary>
        public string resource_upload_user { get; set; } // 资源上传人

        /// <summary>
        /// 上传用户的MongoDB UserInfo.Username属性值
        /// 
        /// 因为常用，所以固化AeInfo在数据中
        /// </summary>
        public string resource_upload_username { get; set; } // 资源上传人

        /// <summary>
        /// 资源评分，小数，最高5分，最低0分
        /// </summary>
        public Double resource_score { get; set; }

        /// <summary>
        /// 资源下载次数
        /// </summary>
        public Int64 resource_download_count { get; set; }  // 下载次数

        /// <summary>
        /// 资源播放次数
        /// </summary>
        public Int64 resource_view_count { get; set; } // 浏览次数

        /// <summary>
        /// 视频清晰度，任意字符串
        /// </summary>
        public string ae_clarity { get; set; } // 清晰度，任意字符串

        /// <summary>
        /// 未找到时，返回example.zip
        /// </summary>
        public string ae_download_url { get; set; } // 下载URL

        /// <summary>
        /// AE资源的总秒数
        /// </summary>
        public Int64 ae_duration { get; set; } // 总秒数

        /// <summary>
        /// AE资源使用的插件名称列表
        /// </summary>
        public List<string> ae_plugins { get; set; } // 使用的插件名称列表

        /// <summary>
        /// AE资源的标题
        /// </summary>
        public string ae_title { get; set; } // 标题

        /// <summary>
        /// 未找到时，返回example.mp4，预览的视频格式都是MP4
        /// </summary>
        public string ae_url { get; set; } // 预览URL

        /// <summary>
        /// 版本号，任意字符串
        /// </summary>
        public string ae_version { get; set; } // 版本号

        public AeInfoJSON()
        {
            id = "";
            resource_type = "视频资源";
            resource_lang = new List<string>();
            resource_keyword = new List<string>();
            resource_tag = new List<string>();
            resource_publish_date = "1900-01-01T00:00:00.000+00:00";
            resource_description = "";
            resource_source_uri = "";
            resource_file_name = "";
            resource_file_name_zh = "";
            resource_file_extension = ".mp4";
            resource_file_size_string = "";
            resource_upload_date = "1900-01-01T00:00:00.000+00:00";
            resource_upload_user = "";
            resource_upload_username = "";
            resource_score = 0;
            resource_download_count = 0;
            resource_view_count = 0;

            ae_clarity = "";
            ae_download_url = "";
            ae_duration = 0;
            ae_plugins = new();
            ae_title = "";
            ae_url = "";
            ae_version = "";
        }

        public AeInfoJSON(AeInfo ae)
        {
            id = ae.Id.ToString();
            resource_type = ae.resource_type;
            resource_lang = ae.resource_lang;
            resource_keyword = ae.resource_keyword;
            resource_tag = ae.resource_tag;
            resource_publish_date = new DateTimeOffset(ae.resource_publish_date.ToUniversalTime()).ToString();
            resource_description = ae.resource_description;
            resource_source_uri = ae.resource_source_uri;
            resource_file_name = ae.resource_file_name;
            resource_file_name_zh = ae.resource_file_name_zh;
            resource_file_extension = ae.resource_file_extension;
            resource_file_size_string = ae.resource_file_size_string;
            resource_upload_date = new DateTimeOffset(ae.resource_upload_date.ToUniversalTime()).ToString();
            resource_upload_user = ae.resource_upload_user.ToString();
            resource_upload_username = ae.resource_upload_username;
            resource_score = ae.resource_score; // 最低0，最高5，double
            resource_view_count = ae.resource_view_count;
            resource_download_count = ae.resource_download_count;

            ae_clarity = ae.ae_clarity;
            ae_duration = ae.ae_duration;
            ae_plugins = ae.ae_plugins;
            ae_title = ae.ae_title;
            ae_version = ae.ae_version;

            ae_url = /*resource*/$"https://yxcqsource.oss-cn-beijing.aliyuncs.com/temp/{id}.mp4";
            var _httpClient = new HttpClient();
            var response = _httpClient.Send(new HttpRequestMessage(HttpMethod.Head, ae_url));
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ae_url = /*resource*/$"https://yxcqsource.oss-cn-beijing.aliyuncs.com/temp/example.mp4";
            }

            ae_download_url = /*download*/$"https://yxcqsource.oss-cn-beijing.aliyuncs.com/temp/{id}.zip";
            response = _httpClient.Send(new HttpRequestMessage(HttpMethod.Head, ae_download_url));
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ae_download_url = /*download*/$"https://yxcqsource.oss-cn-beijing.aliyuncs.com/temp/example.zip";
            }
        }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json.Serialization;
using System.Globalization;
using OfficeOpenXml;

namespace LCAPI.Models
{
    /// <summary>
    /// helper class for full text search
    /// </summary>
    public class NormalSearch
    {
        public string SearchText { get; set; }
    }

    /// <summary>
    /// helper class for search
    /// xx1 xx2 xx3, except PublicDate
    /// if null or empty , ignore relate property(col)
    /// </summary>
    public class Search
    {
        /// <summary>
        /// yyyy.mm.dd-yyyy.mm.dd
        /// </summary>
        public string PublicDate { get; set; }
        public string Keywords { get; set; }
        public string Langs { get; set; }
        public string Areas { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
    }


    /// <summary>
    /// media info, for now, all properties fit video resource 
    /// </summary>
    [BsonIgnoreExtraElements]
    public class MediaInfo
    {
        /// <summary>
        /// mongoDB Collention Name
        /// </summary>
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

        /// <summary>
        /// mongodb id
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        /// <summary>
        /// resource type, for now, only have "video"
        /// </summary>
        public string resource_type { get; set; } // 资源类型  
        public string resource_lang { get; set; } // 语种
        /// <summary>
        /// split by " "
        /// </summary>
        public string resource_keyword { get; set; } // 关键字
        /// <summary>
        /// split by " "
        /// </summary>
        public string resource_tag { get; set; }// 标签（领域）
        /// <summary>
        /// yyyy.MM.dd
        /// </summary>
        public string resource_publish_date { get; set; } // 资源产生时间
        public string resource_description { get; set; } // 资源描述
        public string resource_source_uri { get; set; } // 资源来源

        public string resource_file_name { get; set; }// 原始文件名
        public string resource_file_name_zh { get; set; }// 中文文件名
        /// <summary>
        /// "mp4", extension without point "."
        /// </summary>
        public string resource_file_extension { get; set; } // 资源文件后缀
        public string resource_file_size { get; set; }// 文件大小
        /// <summary>
        /// yyyy.MM.dd
        /// </summary>
        public string resource_upload_date { get; set; }// 资源上传时间
        /// <summary>
        /// user id
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string resource_upload_user { get; set; } // 资源上传人
        public string resource_upload_username { get; set; } // 资源上传人

        public string video_clarity { get; set; } // 视频清晰度
        /// <summary>
        /// hh:mm:ss
        /// </summary>
        public string video_duration { get; set; } // 视频时长

        public static List<MediaInfo> FullTextFind(List<string> keywords)
        {
            return MediaInfo.DBCollation.AsQueryable().Where(t => keywords.All(kw => t.resource_description.Contains(kw))
            || keywords.All(kw => t.resource_file_name.Contains(kw))
            || keywords.All(kw => t.resource_file_name_zh.Contains(kw))
            || keywords.All(kw => t.resource_keyword.Contains(kw))
            || keywords.All(kw => t.resource_tag.Contains(kw))).ToList();
        }

        public static List<MediaInfo> Search(Search search)
        {
            // 视频发布时间范围，如果转换失败，默认起始结束时间都是当前
            var startPublishDate = DateTime.Now;
            var endPublishDate = DateTime.Now;
            var startPublishDateConvert = DateTime.TryParseExact(search.PublicDate.Split('-')[0], "yyyy.M.d", CultureInfo.InvariantCulture, DateTimeStyles.None, out startPublishDate);
            var endPublishDateConvert = false;
            if (startPublishDateConvert == true)
            {
                endPublishDateConvert = DateTime.TryParseExact(search.PublicDate.Split('-')[1], "yyyy.M.d", CultureInfo.InvariantCulture, DateTimeStyles.None, out endPublishDate);
            }

            // 关键词
            var keywords = search.Keywords.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 语种
            var langs = search.Langs.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 领域
            var areas = search.Areas.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 描述
            var deses = search.Description.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 文件名
            var fns = search.FileName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var filter = MediaInfo.DBCollation.AsQueryable().Where(t=>true);

            if(keywords.Count() > 0)
            {
                filter = filter.Where(t => keywords.All(kw => t.resource_keyword.Contains(kw)));
            }

            if (langs.Count() > 0)
            {
                filter = filter.Where(t=> langs.All(l=>t.resource_lang.Contains(l)));
            }

            if (areas.Count() > 0)
            {
                filter = filter.Where(t => areas.All(a => t.resource_tag.Contains(a)));
            }

            if (deses.Count() > 0)
            {
                filter = filter.Where(t => deses.All(d => t.resource_description.Contains(d)));
            }

            if (fns.Count() > 0)
            {
                filter = filter.Where(t => fns.All(f => t.resource_file_name.Contains(f) || t.resource_file_name_zh.Contains(f)));
            }

            var res = filter.ToList();

            if (startPublishDateConvert == true && endPublishDateConvert == true)
            {
                res = res.Where(t =>
                {
                    var publishDate = DateTime.Now;
                    var tranres = DateTime.TryParseExact(t.resource_publish_date, "yyyy.M.d", CultureInfo.InvariantCulture, DateTimeStyles.None, out publishDate);
                    if(tranres == true)
                        if (publishDate < startPublishDate || publishDate > endPublishDate) 
                            return false;
                    return true;
                }).ToList();
            }

            return res;
        }

        public static string UpdateDBFromExcel(string filePath, UserInfo user)
        {
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
                        var mediaInfo = new MediaInfo();
                        mediaInfo.Id = worksheet.Cells[rowindex, 1].Value?.ToString() ?? "";
                        mediaInfo.resource_type = worksheet.Cells[rowindex, 2].Value?.ToString() ?? "";
                        mediaInfo.resource_lang = worksheet.Cells[rowindex, 3].Value?.ToString() ?? "";
                        mediaInfo.resource_keyword = worksheet.Cells[rowindex, 4].Value?.ToString() ?? "";
                        mediaInfo.resource_tag = worksheet.Cells[rowindex, 5].Value?.ToString() ?? "";
                        mediaInfo.resource_publish_date = worksheet.Cells[rowindex, 6].Value?.ToString() ?? "";
                        mediaInfo.resource_description = worksheet.Cells[rowindex, 7].Value?.ToString() ?? "";
                        mediaInfo.resource_source_uri = worksheet.Cells[rowindex, 8].Value?.ToString() ?? "";
                        mediaInfo.resource_file_name = worksheet.Cells[rowindex, 9].Value?.ToString() ?? "";
                        mediaInfo.resource_file_name_zh = worksheet.Cells[rowindex, 10].Value?.ToString() ?? "";
                        mediaInfo.resource_file_extension = worksheet.Cells[rowindex, 11].Value?.ToString() ?? "";
                        mediaInfo.resource_file_size = worksheet.Cells[rowindex, 12].Value?.ToString() ?? "";
                        mediaInfo.resource_upload_date = DateTime.Now.ToString("yyyy.M.d");
                        mediaInfo.resource_upload_user = user.Id;
                        mediaInfo.resource_upload_username = user.Username;
                        mediaInfo.video_clarity = worksheet.Cells[rowindex, 16].Value?.ToString() ?? "";
                        mediaInfo.video_duration = worksheet.Cells[rowindex, 17].Value?.ToString() ?? "";

                        var id = ObjectId.Empty;
                        var idres = ObjectId.TryParse(mediaInfo.Id, out id);
                        if (idres)
                            MediaInfo.DBCollation.DeleteOne(Builders<MediaInfo>.Filter.Eq(t => t.Id, mediaInfo.Id));
                        else
                            mediaInfo.Id = "";
                        MediaInfo.DBCollation.InsertOne(mediaInfo);
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "";
        }
    }
}

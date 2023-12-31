﻿using MongoDB.Bson.Serialization.Attributes;
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

        [BsonId]
        public ObjectId Id { get; set; }
        public string resource_type { get; set; } // 资源类型，固定 视频资源
        public List<String> resource_lang { get; set; } // 语种
        public List<String> resource_keyword { get; set; } // 关键字
        public List<String> resource_tag { get; set; }// 标签（领域）

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime resource_publish_date { get; set; } // 资源产生时间，Local UTC+8
        public string resource_description { get; set; } // 资源描述
        public string resource_source_uri { get; set; } // 资源来源
        public string resource_file_name { get; set; }// 原始文件名
        public string resource_file_name_zh { get; set; }// 中文文件名
        public string resource_file_extension { get; set; } // 资源文件后缀，固定.mp4
        public String resource_file_size_string { get; set; }// 文件大小，MB

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime resource_upload_date { get; set; }// 资源上传时间，Local UTC+8
        public ObjectId resource_upload_user { get; set; } // 资源上传人
        public string resource_upload_username { get; set; } // 资源上传人
        public string video_clarity { get; set; } // 视频清晰度
        public Int64 video_duration { get; set; } // 视频时长，秒

        public double resource_score { get; set; } // 资源评分，最低0分，最高5分

        public Int64 resource_download_count { get; set; }  // 下载次数

        public Int64 resource_view_count { get; set; } // 浏览次数

        public List<string> video_equipment_tag { get; set; } // 装备分类

        public MediaInfo()
        {
            Id = ObjectId.Empty;
            resource_type = "视频资源";
            resource_lang = new List<string>();
            resource_keyword = new List<string>();
            resource_tag = new List<string>();
            resource_publish_date = DateTime.Now; // UTC+8
            resource_description = "";
            resource_source_uri = "";
            resource_file_name = "";
            resource_file_name_zh = "";
            resource_file_extension = ".mp4";
            resource_file_size_string = "";
            resource_upload_date = DateTime.Now; // UTC+8
            resource_upload_user = ObjectId.Empty;
            resource_upload_username = "";
            video_clarity = "";
            video_duration = 0;
            resource_score = 0;
            resource_download_count = 0;
            resource_view_count = 0;
            video_equipment_tag = new List<string>();
        }

        public MediaInfo(MediaInfoJSON json)
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
            video_clarity = json.video_clarity;
            video_duration = json.video_duration;
            resource_score = json.resource_score;
            resource_download_count = json.resource_download_count;
            resource_view_count = json.resource_view_count;
            video_equipment_tag = json.video_equipment_tag;
        }

        public MediaInfoJSON ToMediaInfoJSON()
        {
            return new MediaInfoJSON(this);
        }

        public static List<MediaInfo> FullTextFind(List<string> keywords, int page, int page_size, string order, out int total_count)
        {
            var query = MediaInfo.DBCollation.AsQueryable();

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
                                    || keywords.All(kw => t.video_equipment_tag.Contains(kw)));
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

        public static List<MediaInfo> Search(SearchJSON search, int page, int page_size, string order, out int total)
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

            var query = MediaInfo.DBCollation.AsQueryable();

            if (search.resource_keyword.Count > 0)
            {
                query = query.Where(t => search.resource_keyword.All(kw => t.resource_keyword.Contains(kw)));
            }

            if(search.video_equipment_tag.Count > 0)
            {
                query = query.Where(t => search.video_equipment_tag.All(kw => t.video_equipment_tag.Contains(kw)));
            }

            if (search.resource_lang.Count > 0)
            {
                query = query.Where(t => search.resource_lang.All(l => t.resource_lang.Contains(l)));
            }

            if (search.resource_tag.Count > 0)
            {
                query = query.Where(t => search.resource_tag.All(a => t.resource_tag.Contains(a)));
            }

            if (search.resource_description.Count > 0)
            {
                query = query.Where(t => search.resource_description.All(d => t.resource_description.Contains(d)));
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

        public static MediaInfo? UpdateScore(string id, double value)
        {
            var filter = Builders<MediaInfo>.Filter
                .Eq(t => t.Id, new ObjectId(id));
            var update = Builders<MediaInfo>.Update
                .Set(t => t.resource_score, value);
            var res = MediaInfo.DBCollation.UpdateOne(filter, update);
            return MediaInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == new ObjectId(id));
        }

        public static MediaInfo? UpdateDownloadCount(string id)
        {
            var old = MediaInfo.DBCollation.AsQueryable().Where(t => t.Id == new ObjectId(id)).FirstOrDefault();
            var oldValue = old == null ? 0 : old.resource_download_count;

            var filter = Builders<MediaInfo>.Filter
                .Eq(t => t.Id, new ObjectId(id));
            var update = Builders<MediaInfo>.Update
                .Set(t => t.resource_download_count, oldValue + 1);
            var res = MediaInfo.DBCollation.UpdateOne(filter, update);
            return MediaInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == new ObjectId(id));
        }

        public static MediaInfo? UpdateViewCount(string id)
        {
            var old = MediaInfo.DBCollation.AsQueryable().Where(t => t.Id == new ObjectId(id)).FirstOrDefault();

            var oldValue = old == null ? 0 : old.resource_view_count;

            var filter = Builders<MediaInfo>.Filter.Eq(t => t.Id, new ObjectId(id));
            var update = Builders<MediaInfo>.Update.Set(t => t.resource_view_count, oldValue + 1);
            var res = MediaInfo.DBCollation.UpdateOne(filter, update);
            return MediaInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == new ObjectId(id));
        }

        public static MediaInfo? Insert(MediaInfo media)
        {
            var filter = Builders<MediaInfo>.Filter.Eq(t => t.Id, media.Id);

            if (filter != null)
                MediaInfo.DBCollation.DeleteOne(filter);

            MediaInfo.DBCollation.InsertOne(media);
            return MediaInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == media.Id);
        }

        public static MediaInfo? Update(MediaInfo media)
        {
            var filter = Builders<MediaInfo>.Filter.Eq(t => t.Id, media.Id);

            if (filter != null)
                MediaInfo.DBCollation.DeleteOne(filter);

            MediaInfo.DBCollation.InsertOne(media);
            return MediaInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == media.Id);
        }

        public static void Delete(string id)
        {
            var filter = Builders<MediaInfo>.Filter.Eq(t => t.Id, new ObjectId(id));
            var res = MediaInfo.DBCollation.DeleteOne(filter);
        }

        public static MediaInfo? GetMediaInfoById(string id)
        {
            return MediaInfo.DBCollation.AsQueryable().FirstOrDefault(t => t.Id == new ObjectId(id));
        }

        public static void UpdateDBFromExcel(string filePath, UserInfo user, bool ignoreError,out string errorString, out List<MediaInfo> newMedias, out List<int> newErrorMediasRowIndex)
        {
            errorString = "";
            newMedias = new List<MediaInfo>();
            newErrorMediasRowIndex = new List<int>();

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
                            var mediaInfo = new MediaInfo();
                            mediaInfo.Id = ObjectId.Empty;
                            mediaInfo.resource_type = "视频资源";

                            mediaInfo.resource_lang = (worksheet.Cells[rowindex, 1].Value?.ToString() ?? "").Split(' ').ToList();

                            mediaInfo.resource_keyword = (worksheet.Cells[rowindex, 2].Value?.ToString() ?? "").Split(' ').ToList();

                            mediaInfo.resource_tag = (worksheet.Cells[rowindex, 3].Value?.ToString() ?? "").Split(' ').ToList();

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
                            mediaInfo.resource_publish_date = publishDate;

                            mediaInfo.resource_description = worksheet.Cells[rowindex, 5].Value?.ToString() ?? "";
                            mediaInfo.resource_source_uri = worksheet.Cells[rowindex, 6].Value?.ToString() ?? "";
                            mediaInfo.resource_file_name = worksheet.Cells[rowindex, 7].Value?.ToString() ?? "";
                            mediaInfo.resource_file_name_zh = worksheet.Cells[rowindex, 8].Value?.ToString() ?? "";
                            mediaInfo.resource_file_extension = ".mp4";
                            mediaInfo.resource_file_size_string = worksheet.Cells[rowindex, 9].Value?.ToString() ?? "";

                            mediaInfo.resource_upload_date = DateTime.Now;
                            mediaInfo.resource_upload_user = user.Id;
                            mediaInfo.resource_upload_username = user.username;
                            mediaInfo.video_clarity = worksheet.Cells[rowindex, 10].Value?.ToString() ?? "";

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
                            mediaInfo.video_duration = hour * 3600 + minute * 60 + second;
                            if (durationRes == false && ignoreError == true)
                            {
                                errorString += $"row_{rowindex}_warring: video_duration format error\r\n\r\n";
                            }
                            else if (durationRes == false && ignoreError == false)
                            {
                                throw new Exception("video_duration format error");
                            }

                            mediaInfo.resource_score = 0;
                            mediaInfo.resource_download_count = 0;
                            mediaInfo.resource_view_count = 0;

                            mediaInfo.video_equipment_tag = (worksheet.Cells[rowindex, 12].Value?.ToString() ?? "").Split(' ').ToList();

                            MediaInfo.DBCollation.InsertOne(mediaInfo);
                            newMedias.Add(mediaInfo);
                        }
                        catch (Exception rowEx)
                        {
                            errorString += $"row_{rowindex}_error: {rowEx.Message}\r\n\r\n";
                            newErrorMediasRowIndex.Add(rowindex);
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
    public class MediaInfoJSON
    {
        /// <summary>
        /// MongoDB 内置的主键，一串随机字符
        /// 创建新的实例时，可以不填，由MongoDB自动生成
        /// 查找、更新、删除时，可以使用该列进行数据定位
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 固定值：“视频资源”
        /// </summary>
        public string resource_type { get; set; } // 资源类型  

        /// <summary>
        /// 语种信息，可以为多语种
        /// </summary>
        public List<String> resource_lang { get; set; } // 语种

        /// <summary>
        /// 关键词信息
        /// </summary>
        public List<String> resource_keyword { get; set; } // 关键字

        /// <summary>
        /// 领域
        /// </summary>
        public List<String> resource_tag { get; set; }// 标签（领域）

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
        /// 原始资源名（外文），任意字符串
        /// </summary>
        public string resource_file_name { get; set; }// 原始文件名

        /// <summary>
        /// 中文资源名称，任意字符串
        /// </summary>
        public string resource_file_name_zh { get; set; }// 中文文件名


        /// <summary>
        /// 固定值：“.mp4”，小写字母
        /// </summary>
        public string resource_file_extension { get; set; } // 资源文件后缀

        /// <summary>
        /// 任意字符串，请标明单位。例：1KB，2.5MB，5.9GB，7TB，10PB
        /// </summary>
        public String resource_file_size_string { get; set; }// 文件大小

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
        /// 因为常用，所以固化MediaInfo在数据中
        /// </summary>
        public string resource_upload_username { get; set; } // 资源上传人

        /// <summary>
        /// 视频清晰度，任意字符串
        /// </summary>
        public string video_clarity { get; set; } // 视频清晰度

        /// <summary>
        /// 视频长度，单位为秒，实际类型为Int64
        /// </summary>
        public Int64 video_duration { get; set; } // 视频时长

        /// <summary>
        /// 视频URL，GET，如果服务器中没有该视频，播放的是example.mp4
        /// </summary>
        public string video_url { get; set; }

        /// <summary>
        /// 视频下载URL，GET，如果服务器中没有该视频，下载的是example.mp4
        /// </summary>
        public string video_download_url { get; set; }

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
        /// 装备分类
        /// </summary>
        public List<string> video_equipment_tag { get; set; } // 装备分类

        public MediaInfoJSON()
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
            video_clarity = "";
            video_duration = 0;
            video_url = "";
            video_download_url = "";
            resource_score = 0;
            resource_download_count = 0;
            resource_view_count = 0;
            video_equipment_tag = new List<string>();
        }

        public MediaInfoJSON(MediaInfo media)
        {
            id = media.Id.ToString();
            resource_type = media.resource_type;
            resource_lang = media.resource_lang;
            resource_keyword = media.resource_keyword;
            resource_tag = media.resource_tag;
            resource_publish_date = new DateTimeOffset(media.resource_publish_date.ToUniversalTime()).ToString();
            resource_description = media.resource_description;
            resource_source_uri = media.resource_source_uri;
            resource_file_name = media.resource_file_name;
            resource_file_name_zh = media.resource_file_name_zh;
            resource_file_extension = media.resource_file_extension;
            resource_file_size_string = media.resource_file_size_string;
            resource_upload_date = new DateTimeOffset(media.resource_upload_date.ToUniversalTime()).ToString();
            resource_upload_user = media.resource_upload_user.ToString();
            resource_upload_username = media.resource_upload_username;
            video_clarity = media.video_clarity;
            video_duration = media.video_duration; ; // 实际为Int64

            video_url = /*resource*/$"https://yxcqsource.oss-cn-beijing.aliyuncs.com/temp/{id}.mp4";
            video_download_url = /*download*/$"https://yxcqsource.oss-cn-beijing.aliyuncs.com/temp/{id}.mp4";

            var _httpClient = new HttpClient();
            var response = _httpClient.Send(new HttpRequestMessage(HttpMethod.Head, video_url));
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                video_url = /*resource*/$"https://yxcqsource.oss-cn-beijing.aliyuncs.com/temp/example.mp4";
                video_download_url = /*download*/$"https://yxcqsource.oss-cn-beijing.aliyuncs.com/temp/example.mp4";
            }

            resource_score = media.resource_score; // 最低0，最高5，double
            resource_view_count = media.resource_view_count;
            resource_download_count = media.resource_download_count;
            video_equipment_tag = media.video_equipment_tag;
        }
    }
}

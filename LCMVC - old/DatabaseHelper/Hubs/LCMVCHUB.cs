using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;

namespace LCMVC.DatabaseHelper.Hubs
{
    public class LCMVCHUB: Hub
    {
        public LCMVCHUB()
        {

        }

        public async Task AdminIndexPageInsertMedia(string sendjson)
        {
            // verify
            if (sendjson == null || sendjson == "")
            {
                await Clients.Caller.SendAsync("AdminIndexPageInsertMedia", "空的JSON字符串");
                return;
            }

            Dictionary<string, string>? dictionary = null;

            try
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(sendjson);
            }
            catch
            {
                dictionary = null;
            }

            if (dictionary == null || dictionary.Count == 0)
            {
                await Clients.Caller.SendAsync("AdminIndexPageInsertMedia", "JSON字符串中不包含任何数据");
                return;
            }

            var newMediaInfo = new MediaInfo();

            // newMediaInfo.Id = new ObjectId();

            var mediaType = dictionary["mediatype"];
            if (mediaType == null || (mediaType != "视频素材" && mediaType != "ae素材") && mediaType != "图片素材" && mediaType != "文档素材")
            {
                await Clients.Caller.SendAsync("AdminIndexPageInsertMedia", "素材类型为空，或为错误选项 " + mediaType ?? "");
            }
            newMediaInfo.mediatype = mediaType;

            newMediaInfo.mediatitle = dictionary["mediatitle"] ?? "未命名";
            newMediaInfo.mediatitle_zh = dictionary["mediatitle_zh"] ?? "未命名";
            newMediaInfo.mediakeyword = dictionary["mediakeyword"] ?? "";
            newMediaInfo.mediaarea = dictionary["mediaarea"] ?? "";

            var playcount = 0;
            int.TryParse(dictionary["mediaplaycount"] ?? "", out playcount);
            newMediaInfo.mediaplaycount = playcount;

            double averstar = 0;
            double.TryParse(dictionary["mediaaverstar"] ?? "", out averstar);
            newMediaInfo.mediaaverstar = averstar;

            newMediaInfo.mediauploaddate = DateTime.Now.ToString("yyyy.MM.dd");
            newMediaInfo.mediauploaduserid = new ObjectId(dictionary["mediauploaduserid"] ?? "");
            newMediaInfo.mediaextension = dictionary["mediaextension"] ?? "";
            newMediaInfo.mediainfo = dictionary["mediainfo"] ?? "";
            newMediaInfo.mediapath = dictionary["mediapath"] ?? "";
            newMediaInfo.mediapublishdate = dictionary["mediapublishdate"] ?? "";
            newMediaInfo.mediaquality = dictionary["mediaquality"] ?? "";
            newMediaInfo.mediatime = dictionary["mediatime"] ?? "";

            MediaInfo.DBCollation.InsertOne(newMediaInfo);

            await Clients.Caller.SendAsync("AdminIndexPageInsertMedia", "");
        }

        public async Task AdminIndexPageUpdateMedia(string sendjson)
        {
            // verify
            if (sendjson == null || sendjson == "")
            {
                await Clients.Caller.SendAsync("AdminIndexPageUpdateMedia", "空的JSON字符串");
                return;
            }

            Dictionary<string, string>? dictionary = null;

            try
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(sendjson);
            }
            catch
            {
                dictionary = null;
            }

            if (dictionary == null || dictionary.Count == 0)
            {
                await Clients.Caller.SendAsync("AdminIndexPageUpdateMedia", "JSON字符串中不包含任何数据");
                return;
            }

            var mediaId = dictionary["mediaId"] ?? "";
            if (MediaInfo.DBCollation.AsQueryable().Where(t => t.Id.ToString() == mediaId).Count() <= 0)
            {
                await Clients.Caller.SendAsync("AdminIndexPageUpdateMedia", "未找到更新的数据");
                return;
            }

            var newMediaInfo = new MediaInfo();

            newMediaInfo.Id = new ObjectId(mediaId);

            var mediaType = dictionary["mediatype"];
            if (mediaType == null || (mediaType != "视频素材" && mediaType != "ae素材") && mediaType != "图片素材" && mediaType != "文档素材")
            {
                await Clients.Caller.SendAsync("AdminIndexPageUpdateMedia", "素材类型为空，或为错误选项 " + mediaType ?? "");
            }
            newMediaInfo.mediatype = mediaType;

            newMediaInfo.mediatitle = dictionary["mediatitle"] ?? "未命名";
            newMediaInfo.mediatitle_zh = dictionary["mediatitle_zh"] ?? "未命名";
            newMediaInfo.mediakeyword = dictionary["mediakeyword"] ?? "";
            newMediaInfo.mediaarea = dictionary["mediaarea"] ?? "";

            var playcount = 0;
            int.TryParse(dictionary["mediaplaycount"] ?? "", out playcount);
            newMediaInfo.mediaplaycount = playcount;

            double averstar = 0;
            double.TryParse(dictionary["mediaaverstar"] ?? "", out averstar);
            newMediaInfo.mediaaverstar = averstar;

            newMediaInfo.mediauploaddate = DateTime.Now.ToString("yyyy.MM.dd");
            newMediaInfo.mediauploaduserid = new ObjectId(dictionary["mediauploaduserid"] ?? "");
            newMediaInfo.mediaextension = dictionary["mediaextension"] ?? "";
            newMediaInfo.mediainfo = dictionary["mediainfo"] ?? "";
            newMediaInfo.mediapath = dictionary["mediapath"] ?? "";
            newMediaInfo.mediapublishdate = dictionary["mediapublishdate"] ?? "";
            newMediaInfo.mediaquality = dictionary["mediaquality"] ?? "";
            newMediaInfo.mediatime = dictionary["mediatime"] ?? "";

            MediaInfo.DBCollation.DeleteOne(Builders<MediaInfo>.Filter.Eq("_id", newMediaInfo.Id));
            MediaInfo.DBCollation.InsertOne(newMediaInfo);

            await Clients.Caller.SendAsync("AdminIndexPageUpdateMedia", "");
        }

        public async Task AdminIndexPageDeleteMedia(string id)
        {
            var res = MediaInfo.DBCollation.DeleteOne(Builders<MediaInfo>.Filter.Eq("_id", new ObjectId(id)));
            await Clients.Caller.SendAsync("AdminIndexPageDeleteMedia", "");
        }

        public async Task SearchPageForMediaList(string sendjson)
        {
            // verify
            if (sendjson == null || sendjson == "")
            {
                await Clients.Caller.SendAsync("SearchPageForMediaList", "");
                return;
            }

            Dictionary<string, string>? dictionary = null;

            try
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(sendjson);
            }
            catch
            {
                dictionary = null;
            }
            
            if (dictionary == null || dictionary.Count == 0)
            {
                await Clients.Caller.SendAsync("SearchPageForMediaList", "");
                return;
            }

            var mediaType_video = dictionary["mediaType_video"];
            var mediaType_ae = dictionary["mediaType_ae"];
            var mediaType_image = dictionary["mediaType_image"];
            var mediaType_document = dictionary["mediaType_document"];
            var titleInputValue = dictionary["titleInputValue"];
            var publishdateValue = dictionary["publishdateValue"];
            var keywordsValue = dictionary["keywordsValue"];
            var areaselectedValue = dictionary["areaselectedValue"];

            // 视频类型
            var mediaType = new List<string>();
            if (mediaType_video != null && mediaType_video == "true") mediaType.Add("视频素材");
            if (mediaType_ae != null && mediaType_ae == "true") mediaType.Add("ae素材");
            if (mediaType_image != null && mediaType_image == "true") mediaType.Add("图片素材");
            if (mediaType_document != null && mediaType_document == "true") mediaType.Add("文档素材");

            // 关键词
            var keywords = keywordsValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 视频发布时间范围，如果转换失败，默认起始结束时间都是当前
            var startPublishDate = DateTime.Now;
            var endPublishDate = DateTime.Now;
            var startPublishDateConvert = DateTime.TryParseExact(publishdateValue.Split('-')[0], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startPublishDate);
            var endPublishDateConvert = false;
            if (startPublishDateConvert == true)
            {
                endPublishDateConvert = DateTime.TryParseExact(publishdateValue.Split('-')[1], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endPublishDate);
            }

            // 领域，使用的是领域索引号
            var areaIndex = areaselectedValue.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // MongoDB 拉取所有类型
            // MongoDB.Where 不支持函数查询
            // LINQ.Where 继续后续查询
            var list = MediaInfo.DBCollation.AsQueryable().Where(t => mediaType.Contains(t.mediatype)).ToList().Where(t => {
                // 如果有“标题”查询，查询“原标题”和“中文标题”
                if (titleInputValue!= null && titleInputValue != "")
                    if (!t.mediatitle.Contains(titleInputValue) || !t.mediatitle.Contains(titleInputValue))
                        return false;

                // 如果有“发布起止时间“，查询”发布时间”列
                if (startPublishDateConvert == true && endPublishDateConvert == true)
                {
                    var publishDate = DateTime.ParseExact(t.mediapublishdate, "yyyy.M.d", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    if (publishDate < startPublishDate || publishDate > endPublishDate) return false;
                }

                // 如果有关键字，查询“关键字”列
                if (keywords.Count() != 0)
                {
                    var findkw = false;
                    foreach (var kw in keywords)
                    {
                        if (t.mediakeyword.Contains(kw))
                        {
                            // 查询到了
                            findkw = true;
                            break;
                        }
                    }
                    // 未查询到
                    if (findkw == false) return false;
                }

                // 如果有领域，查询“领域”列
                if (areaIndex.Count() != 0)
                {
                    var findarea = false;
                    foreach (var area in areaIndex)
                    {
                        if (t.mediaarea.Contains(area))
                        {
                            findarea = true;
                            break;
                        }
                    }
                    if (findarea == false) return false;
                }

                // 以上查询如有任一查询失败，会直接 return false
                return true;
            });

            await Clients.Caller.SendAsync("SearchPageForMediaList", JsonConvert.SerializeObject(list));
        }
    }
}

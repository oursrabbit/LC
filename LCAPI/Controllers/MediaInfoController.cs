using LCAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Net.Http;
using System.Net;
using OfficeOpenXml;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using MongoDB.Driver.GeoJsonObjectModel;
using Newtonsoft.Json.Linq;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml.Style;
using MongoDB.Bson;

namespace LCAPI.Controllers
{
    /// <summary>
    /// 与视频资源相关的API接口
    /// 进行了简单的安全验证
    /// 任何请求都需要添加自定义Request头
    /// LCAPI-MEDIAINFO: "pxq"
    /// </summary>
    [ApiController]
    [Route("/LCAPI/[controller]")]
    public class MediaInfoController : ControllerBase
    {
        private readonly NLog.Logger _logger;
        private string loggerString = "";

        public MediaInfoController(ILogger<MediaInfoController> _)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        private bool verifyHeader()
        {
            return HttpContext.Request.Headers["LCAPI-MEDIAINFO"].Equals("pxq");
        }

        /// <summary>
        /// 对视频资源信息进行全文查找
        /// 
        /// 查找的范围参看FullText结构，目前包括范围：描述，文件名，关键词，领域，装备分类
        /// </summary>
        /// <param name="search">如果为空字符串，表示返回所有数据</param>
        /// <param name="page">字符串：当前所处的分页页码，起始值为1</param>
        /// <param name="page_size">每页包含多少条数据</param> 
        /// <param name="order">正序："asc"，倒序："desc"，目前仅对resource_publish_date排序</param> 
        /// <response code="200">MediaInfo JSON Array</response> 
        /// <response code="403">LCAPI-MEDIAINFO错误</response>
        /// <response code="500">其他未知错误</response>
        [HttpPost("Search/FullText")]
        [Produces("application/json")]
        public ActionResult FullTextResourceSearch(FullTextSearchJSON search, int page, int page_size, string order)
        {
            try
            {
                loggerString = $"searchJSON: {Newtonsoft.Json.JsonConvert.SerializeObject(search)}\r\n\r\n" +
                     $"page: {page}, page_size: {page_size}, order: {order}\r\n\r\n";
                Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");
                if (!verifyHeader())
                {
                    loggerString += "error: 403";
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }

                string OrderType = order == "asc" ? "asc" : "desc";

                var total = 0;
                var res = MediaInfo.FullTextFind(search.searchtext, page, page_size, OrderType, out total);

                var resJson = res.ConvertAll<MediaInfoJSON>(t => t.ToMediaInfoJSON());
                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(resJson);
                var json = new JObject();
                json.Add("total_count", total);
                json.Add("page", page);
                json.Add("page_size", page_size);
                json.Add("medias", JArray.Parse(jsonString));
                var returnJson = Newtonsoft.Json.JsonConvert.SerializeObject(json);

                loggerString += $"return: {returnJson}";
                return new JsonResult(returnJson) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
            finally
            {
                _logger.Info(loggerString);
            }
        }

        /// <summary>
        /// 对视频资源的某个属性进行检索
        /// 多属性、多属性值之间使用且的关系
        /// 如果某个属性对应的JSON数组长度为0，则忽略该属性
        /// </summary>
        /// <param name="search">如果某个属性对应的JSON数组长度为0，则忽略该属性</param>
        /// <param name="page">字符串：当前所处的分页页码，起始值为1</param>
        /// <param name="page_size">每页包含多少条数据</param> 
        /// <param name="order">正序："asc"，倒序："desc"，目前仅对resource_publish_date排序</param> 
        /// <response code="200">MediaInfo JSON Array</response> 
        /// <response code="403">LCAPI-MEDIAINFO错误</response>
        /// <response code="500">其他未知错误</response>
        [HttpPost("Search/Property")]
        [Produces("application/json")]
        public ActionResult PropertySearch(SearchJSON search, int page, int page_size, string order)
        {
            try
            {
                loggerString = $"searchJSON: {Newtonsoft.Json.JsonConvert.SerializeObject(search)}\r\n\r\n" +
$"page: {page}, page_size: {page_size}, order: {order}\r\n\r\n";
                Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");
                if (!verifyHeader())
                {
                    loggerString += "error: 403";
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }

                string OrderType = order == "asc" ? "asc" : "desc";

                var total = 0;
                var res = MediaInfo.Search(search, page, page_size, OrderType, out total);

                var resJson = res.ConvertAll<MediaInfoJSON>(t => t.ToMediaInfoJSON());
                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(resJson);
                var json = new JObject();
                json.Add("total_count", total);
                json.Add("page", page);
                json.Add("page_size", page_size);
                json.Add("medias", JArray.Parse(jsonString));

                var returnJson = Newtonsoft.Json.JsonConvert.SerializeObject(json);

                loggerString += $"return: {returnJson}";
                return new JsonResult(returnJson) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
            finally
            {
                _logger.Info(loggerString);
            }
        }

        /// <summary>
        /// 根据ID获取MediaInfoJSON
        /// </summary>
        /// <param name="id">mediainfo.id</param>
        /// <response code="200">MediaInfoJSON</response> 
        /// <response code="403">LCAPI-MEDIAINFO错误</response>
        /// <response code="404">没有找到，id为空</response> 
        /// <response code="500">其他未知错误</response>
        [HttpGet("Search/Media")]
        [Produces("application/json")]
        public ActionResult GetMediaInfoById(string id)
        {
            try
            {
                loggerString = $"id: {id}\r\n\r\n";
                Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");
                if (!verifyHeader())
                {
                    loggerString += "error: 403";
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }
                var media = MediaInfo.GetMediaInfoById(id);

                if (media == null)
                {
                    loggerString += $"error: 403";
                    return RestResultJSON.CreateRestJSONResult("404", "未找到", new MediaInfoJSON(), 404);
                }
                else
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(new MediaInfoJSON(media));


                    loggerString += $"return: {json}";
                    return new JsonResult(json) { StatusCode = 200 };
                }
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
            finally
            {
                _logger.Info(loggerString);
            }
        }

        /// <summary>
        /// 资源评分，最高5分，最低0分，小数
        /// </summary>
        /// <param name="id">mediainfo.id</param>
        /// <param name="score">mediainfo.resource_score</param>
        /// <response code="200">返回更新后的mediaInfo</response> 
        /// <response code="403">LCAPI-MEDIAINFO错误</response>
        /// <response code="404">没有找到，id为空</response> 
        /// <response code="500">其他未知错误</response>
        [HttpGet("Update/Score")]
        public ActionResult UpdateScoreByID(string id, double score)
        {
            try
            {
                loggerString = $"id: {id}\r\n\r\n" + $"score: {score}\r\n\r\n";
                Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");
                if (!verifyHeader())
                {
                    loggerString += $"error: 403";
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }
                var media = MediaInfo.UpdateScore(id, score);

                if (media == null)
                {
                    loggerString += $"error: 404";
                    return RestResultJSON.CreateRestJSONResult("404", "未找到", new MediaInfoJSON(), 404);
                }
                else
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(new MediaInfoJSON(media));

                    loggerString += $"return: {json}";
                    return new JsonResult(json) { StatusCode = 200 };
                }
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
            finally
            {
                _logger.Info(loggerString);
            }
        }

        /// <summary>
        /// 更新资源下载次数
        /// </summary>
        /// <param name="id">mediainfo.id</param>
        /// <response code="200">返回更新后的mediaInfo</response> 
        /// <response code="403">LCAPI-MEDIAINFO错误</response>
        /// <response code="404">没有找到，id为空</response> 
        /// <response code="500">其他未知错误</response>
        [HttpGet("Update/DownloadCount")]
        public ActionResult UpdateDownloadCountByID(string id)
        {
            try
            {
                loggerString = $"id: {id}\r\n\r\n";
                Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");
                if (!verifyHeader())
                {
                    loggerString += $"error: 403";
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }
                var media = MediaInfo.UpdateDownloadCount(id);

                if (media == null)
                {
                    loggerString += $"error: 404";
                    return RestResultJSON.CreateRestJSONResult("404", "未找到", new MediaInfoJSON(), 404);
                }
                else
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(new MediaInfoJSON(media));

                    loggerString += $"return: {json}";
                    return new JsonResult(json) { StatusCode = 200 };
                }
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
            finally
            {
                _logger.Info(loggerString);
            }
        }

        /// <summary>
        /// 更新视频资源播放次数
        /// </summary>
        /// <param name="id">mediainfo.id</param>
        /// <response code="200">返回更新后的mediaInfo</response> 
        /// <response code="403">LCAPI-MEDIAINFO错误</response>
        /// <response code="404">没有找到，id为空</response> 
        /// <response code="500">其他未知错误</response>
        [HttpGet("Update/ViewCount")]
        public ActionResult UpdateViewCountByID(string id)
        {
            try
            {
                loggerString = $"id: {id}\r\n\r\n";
                Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");
                if (!verifyHeader())
                {
                    loggerString += $"error: 403";
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }
                var media = MediaInfo.UpdateViewCount(id);

                if (media == null)
                {
                    loggerString += $"error: 404";
                    return RestResultJSON.CreateRestJSONResult("404", "未找到", new MediaInfoJSON(), 404);
                }
                else
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(new MediaInfoJSON(media));

                    loggerString += $"return: {json}";
                    return new JsonResult(json) { StatusCode = 200 };
                }
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
            finally
            {
                _logger.Info(loggerString);
            }
        }

        /// <summary>
        /// 更新视频资源
        /// 
        /// 实际的数据库操作是，删除再用同样的id创建新的
        /// 
        /// 所有JSON参数都需要填写
        /// </summary>
        /// <param name="mediaJson">mediainfo结构，所有属性都需要填写，删除重建，id不变</param>
        /// <response code="200">返回更新后的mediaInfo</response> 
        /// <response code="403">LCAPI-MEDIAINFO错误</response>
        /// <response code="404">没有找到，id为空</response> 
        /// <response code="500">其他未知错误</response>
        [HttpPost("Update/Media")]
        public ActionResult UpdateMediaInfo(MediaInfoJSON mediaJson)
        {
            loggerString = $"\r\n\r\ncall: UpdateMediaInfoByID\r\n\r\n" + $"mediaJson_new: {Newtonsoft.Json.JsonConvert.SerializeObject(mediaJson)}\r\n\r\n";
            Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");

            try
            {
                if (!verifyHeader())
                {
                    loggerString += $"error: 403";
                    _logger.Info(loggerString);
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }
                var media = MediaInfo.Update(new MediaInfo(mediaJson));

                if (media == null)
                {
                    loggerString += $"error: 404";
                    _logger.Info(loggerString);
                    return RestResultJSON.CreateRestJSONResult("404", "未找到", new MediaInfoJSON(), 404);
                }
                else
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(new MediaInfoJSON(media));

                    loggerString += $"return: {json}";
                    _logger.Info(loggerString);
                    return new JsonResult(json) { StatusCode = 200 };
                }
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                _logger.Info(loggerString);
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
        }

        /// <summary>
        /// 先从GET /Resource/ExcelTemplate/获取的模板，填充数据后，更新数据库
        /// </summary>
        /// <param name="file">根据GET /Resource/ExcelTemplate获取的模板，填写的数据Excel文件</param>
        /// <param name="userId">上传者的userinfo.id字符串</param>
        /// <param name="ignore_cell_error">遇到格式错误的Cell，是否使用默认值覆盖，如果不覆盖，该行将作为ErrorRowIndexList返回</param>
        /// <response code="200">Excel文件打开成功，返回三组数据：上传成功的MediaInfoList，ErrorRowIndexList，ErrorString</response> 
        /// <response code="422">Excel文件处理失败</response>
        /// <response code="403">LCAPI-MEDIAINFO错误</response>
        /// <response code="404">ignore_cell_error = true时，userid未找到</response>
        /// <response code="500">其他未知错误</response>
        [HttpPost("Update/Excel")]
        public async Task<IActionResult> UpdateMediaDBFromExcel(IFormFile file, string userId, bool ignore_cell_error)
        {
            var newGuid = Guid.NewGuid();
            loggerString = $"\r\n\r\ncall: UpdateMediaInfoByID\r\n\r\n" + $"userId: {userId}\r\n\r\n" + $"guid: {newGuid.ToString()}\r\n\r\n";
            Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");

            try
            {
                if (!verifyHeader())
                {
                    loggerString += $"error: 403";
                    _logger.Info(loggerString);
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }
                if (file == null || file.Length == 0)
                {
                    loggerString += $"error: 422";
                    _logger.Info(loggerString);
                    return RestResultJSON.CreateRestJSONResult("422", "Excel not found", "", 422);
                }

                var filePath = $"lc_resource/temp/{newGuid.ToString()}.xlsx";
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var user = UserInfo.GetUserById(userId);
                var errorString = "";

                if (user == null && ignore_cell_error == true)
                {
                    errorString = $"warning: user not found, id: {userId}\r\n\r\n";
                    user = new UserInfo();
                }
                else if(user == null && ignore_cell_error == false)
                {
                    loggerString += $"error: 404 未找到上传用户信息";
                    _logger.Info(loggerString);
                    return RestResultJSON.CreateRestJSONResult("404", "未找到上传用户信息", "", 404);
                }

                var updateErrorString = "";
                var newMediaList = new List<MediaInfo>();
                var errorRowIndexList = new List<int>();
                MediaInfo.UpdateDBFromExcel(filePath, user, ignore_cell_error, out updateErrorString, out newMediaList, out errorRowIndexList);
                errorString += updateErrorString;

                var newMediaListJson = newMediaList.ConvertAll<MediaInfoJSON>(t => t.ToMediaInfoJSON());
                var newMediaListJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(newMediaListJson);

                var errorRowIndexListJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(errorRowIndexList);

                var json = new JObject();
                json.Add("error_string", errorString);
                json.Add("error_row_index", JArray.Parse(errorRowIndexListJsonString));
                json.Add("new_media", JArray.Parse(newMediaListJsonString));
                var returnJson = Newtonsoft.Json.JsonConvert.SerializeObject(json);

                loggerString += $"return: {returnJson}";
                _logger.Info(loggerString);
                return new JsonResult(returnJson) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                _logger.Info(loggerString);
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
        }

        /// <summary>
        /// 删除视频资源
        /// </summary>
        /// <param name="id">mediainfo.id</param>
        /// <response code="200">删除成功</response> 
        /// <response code="403">LCAPI-MEDIAINFO错误</response>
        /// <response code="500">其他未知错误</response>
        [HttpGet("Delete/Media")]
        public ActionResult DeleteMediaInfoByID(string id)
        {
            var newGuid = Guid.NewGuid();
            loggerString = $"\r\n\r\ncall: DeleteMediaInfoByID\r\n\r\n" + $"id: {id}\r\n\r\n";
            Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");

            try
            {
                if (!verifyHeader())
                {
                    loggerString += $"error: 403";
                    _logger.Info(loggerString);
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }
                MediaInfo.Delete(id);

                return Ok();
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                _logger.Info(loggerString);
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
        }

        /// <summary>
        /// 获取数据模板，模板中的列是数据库列的子集
        /// </summary>
        /// <response code="200">Excel文件</response> 
        /// <response code="403">LCAPI-MEDIAINFO错误</response>
        /// <response code="500">其他未知错误</response>
        [HttpGet("Resource/ExcelTemplate")] // ASP.NET Core Web API的文件下载接口地址
        public IActionResult GetExcelTemplate()
        {
            var newGuid = Guid.NewGuid();
            loggerString = $"\r\n\r\ncall: GetExcelTemplate\r\n\r\n";
            Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");

            try
            {
                if (!verifyHeader())
                {
                    loggerString += $"error: 403";
                    _logger.Info(loggerString);
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }

                string filePath = $"lc_resource/media_data.xlsx";
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/octet-stream", "media_data.xlsx");
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                _logger.Info(loggerString);
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
        }

        /// <summary>
        /// 获取数据Excel
        /// </summary>
        /// <param name="ids">mediainfo.id</param>
        /// <response code="200">Excel文件</response> 
        /// <response code="403">LCAPI-MEDIAINFO错误</response>
        /// <response code="500">其他未知错误</response>
        [HttpPost("Resource/GetExcelByIds")] // ASP.NET Core Web API的文件下载接口地址
        public IActionResult GetExcelByIds(List<String> ids)
        {
            loggerString = $"\r\n\r\ncall: GetExcelByIds\r\n\r\n" + $"userInfoJSON: {Newtonsoft.Json.JsonConvert.SerializeObject(ids)}\r\n\r\n";
            Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");

            try
            {
                if (!verifyHeader())
                {
                    loggerString += $"error: 403";
                    _logger.Info(loggerString);
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }

                var filePath = $"lc_resource/temp/out/{Guid.NewGuid().ToString()}.xlsx";
                var medias = new List<MediaInfoJSON>();
                ids.ForEach(t => {
                    var m = MediaInfo.GetMediaInfoById(t);
                    if(m != null)
                    {
                        medias.Add(new MediaInfoJSON(m));
                    }
                });
                    
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Media");
                    var colIndex = 1;
                    typeof(MediaInfoJSON).GetProperties().ToList().ForEach(t =>
                    {
                        worksheet.Cells[1, colIndex].SetCellValue(0, 0, t.Name);
                        for (var i = 0; i < medias.Count; i++)
                        {
                            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(t.GetValue(medias[i]));
                            worksheet.Cells[i + 2, colIndex].SetCellValue(0, 0, jsonString);
                        }
                        colIndex++;
                    });
                    package.SaveAs(filePath);
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/octet-stream", "media_data.xlsx");

            }
            catch (Exception e)
            {
                return NoContent();
            }
        }


        /// <summary>
        /// 总会成功插入数据，错误的属性值会被默认值代替。MediaInfoJSON.id == ""时，插入数据，否则根据id覆盖更新，若未找到则直接插入
        /// </summary>
        /// <param name="media">新的数据</param>
        /// <response code="200">插入成功，并返回带有id的MediaInfoJSON</response> 
        /// <response code="403">LCAPI-MEDIAINFO错误</response>
        /// <response code="500">其他未知错误</response>
        [HttpPost("Insert/MediaInfo")]
        public ActionResult InsertMediaInfo (MediaInfoJSON mediaJson)
        {
            loggerString = $"\r\n\r\ncall: InsertMediaInfo\r\n\r\n" + $"userInfoJSON: {Newtonsoft.Json.JsonConvert.SerializeObject(mediaJson)}\r\n\r\n";
            Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");

            try
            {
                if (!verifyHeader())
                {
                    loggerString += $"error: 403";
                    _logger.Info(loggerString);
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }

                var newMedia = new MediaInfo(mediaJson);
                var insertMedia = MediaInfo.Insert(newMedia);
                insertMedia = insertMedia ?? new MediaInfo();

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(new MediaInfoJSON(insertMedia));

                loggerString += $"return: {json}";
                _logger.Info(loggerString);
                return new JsonResult(json) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                _logger.Info(loggerString);
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
        }
    }
}

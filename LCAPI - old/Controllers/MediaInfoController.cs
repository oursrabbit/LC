using LCAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace LCAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MediaInfoController : ControllerBase
    {
        private readonly ILogger<MediaInfoController> _logger;

        public MediaInfoController(ILogger<MediaInfoController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Full Text search in MediaInfo Model, see C# Model or JS class
        /// </summary>
        /// <param name="search">
        /// if search.searchText = "", return all documents in MongoDB, otherwise, 
        /// will find the keyword, split by " ", in resource_description,resource_file_name，resource_file_name_zh，resource_keyword and resource_tag.
        /// the four parts will be OR relation, while keyword will be AND relation for single column.
        /// </param>
        /// <returns>
        /// MediaInfo List, see C# Model or JS class
        /// </returns>
        [HttpPost("NormalResourceSearch")]
        public string PostNormalResourceSearch(NormalSearch search)
        {
            var res = new List<MediaInfo>();
            if (search.SearchText == null || search.SearchText == "")
            {
                res = MediaInfo.DBCollation.AsQueryable().ToList();
            }
            else
            {
                var keyword = search.SearchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                res = MediaInfo.FullTextFind(keyword.ToList()).ToList();
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(res);
        }

        [HttpPost("ResourceSearch")]
        public string PostResourceSearch(Search search)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(MediaInfo.Search(search));
        }

        /// <summary>
        /// if the video NOT EXIST, using example.mp4 as default video
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/Resource/Video")]
        public async Task<IActionResult> GetVideoResource(string id)
        {
            var filePath = $"lc_resource/video/{id}.mp4";
            if (!System.IO.File.Exists(filePath))
            {
                filePath = $"lc_resource/video/example.mp4";
            }

            var fileInfo = new FileInfo(filePath);
            var fileSize = fileInfo.Length;
            var response = HttpContext.Response;
            response.Headers.Add("Content-Length", fileSize.ToString());
            response.Headers.Add("Accept-Ranges", "bytes");
            response.Headers.Add("Content-Range", $"bytes 0-{fileSize - 1}/{fileSize}");
            response.ContentType = "video/mp4";

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1, true);
            var buffer = new byte[1024 * 1024];
            while (true)
            {
                var bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                await response.Body.WriteAsync(buffer, 0, bytesRead);
                await response.Body.FlushAsync();
            }

            return new EmptyResult();
        }

        [HttpGet("/Resource/Video/ExcelTemplate")] // ASP.NET Core Web API的文件下载接口地址
        public IActionResult GetExcelTemplate()
        {
            string filePath = $"lc_resource/data.xlsx";
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", "data.xlsx");
        }

        [HttpPost("/Resource/Video/UpdateDB")]
        public async Task<IActionResult> UpdateMediaDBFromExcel(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected.");
            }
            // Save the file to a location on the server
            var newGuid = Guid.NewGuid();
            var filePath = $"lc_resource/temp/{newGuid.ToString()}.xlsx";
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(MediaInfo.UpdateDBFromExcel(filePath, UserInfo.GetUserById(userId) ?? new UserInfo() { Id = "", Username = "未知" }));
        }
    }
}

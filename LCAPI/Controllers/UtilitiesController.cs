using LCAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LCAPI.Controllers
{
    /// <summary>
    /// 工具类
    /// </summary>
    [ApiController]
    [Route("/LCAPI/[controller]")]
    public class UtilitiesController : Controller
    {
        private readonly NLog.Logger _logger;
        private string loggerString = "";

        public UtilitiesController(ILogger<UserInfoController> _)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        private bool verifyHeader()
        {
            return HttpContext.Request.Headers["LCAPI-UTILITIES"].Equals("pXQ");
        }

        /// <summary>
        /// 工具软件：将文件名，根据从网站下载的Excel，改为数据库ID编号
        /// flutter 3 windows
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetResourceUploader")]
        public ActionResult GetResourceUploader()
        {
            try
            {
                loggerString = $"GetResourceUploader\r\n\r\n";
                Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");
                if (!verifyHeader())
                {
                    loggerString += "error: 403";
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }
                return Ok(@"http://114.115.220.129:5500/DownLoad/lc_uploader_flutter_windows.zip");
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
        /// 工具软件：生成各种随机数据
        /// .net framework 4.8
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTestDataGenerator")]
        public ActionResult GetTestDataGenerator()
        {
            try
            {
                loggerString = $"GetTestDataGenerator\r\n\r\n";
                Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");
                if (!verifyHeader())
                {
                    loggerString += "error: 403";
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }
                return Ok(@"http://114.115.220.129:5500/DownLoad/lc_datagenerator_windows.zip");
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
    }
}

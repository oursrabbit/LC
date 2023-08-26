using LCAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace LCAPI.Controllers
{
    /// <summary>
    /// 与用户信息相关的API接口
    /// 进行了简单的安全验证
    /// 任何请求都需要添加自定义Request头
    /// LCAPI-USERINFO: "PXQ"
    /// </summary>
    [ApiController]
    [Route("/LCAPI/[controller]")]
    public class UserInfoController : ControllerBase
    {
        private readonly ILogger<UserInfoController> _logger;

        public UserInfoController(ILogger<UserInfoController> logger)
        {
            _logger = logger;
        }

        private bool verifyHeader()
        {
            return HttpContext.Request.Headers["LCAPI-USERINFO"].Equals("PXQ");
        }

        /// <summary>
        /// 用户登录时，通过用户名和密码进行验证
        /// 
        /// 只需要填写username和passwor字段即可
        /// 
        /// 测试系统中，111为普通用户，222为管理员
        /// </summary>
        /// <param name="userLogin">POST DATA：要验证的用户</param>
        /// <response code="200">登录成功，返回用户信息</response>
        /// <response code="401">用户名或密码错误，此时id为空字符</response>
        /// <response code="403">LCAPI-USERINFO错误</response>
        /// <response code="500">其他未知错误</response>
        [HttpPost("Verify/User")]
        [Produces("application/json")]
        public ActionResult VerifyUserInfo(UserInfoJSON userLogin)
        {
            var loggerString = $"\r\n\r\ncall: VerifyUserInfo\r\n\r\n" + $"userInfoJSON: {Newtonsoft.Json.JsonConvert.SerializeObject(userLogin)}\r\n\r\n";
            Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");

            try
            {
                if (!verifyHeader())
                {
                    loggerString += "error: 403";
                    _logger.LogInformation(loggerString);
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }

                UserInfo? user = new UserInfo(userLogin);
                user = UserInfo.VerifyUser(user.username, user.password);
                if (user == null)
                {
                    loggerString += "error: 401";
                    _logger.LogInformation(loggerString);
                    return RestResultJSON.CreateRestJSONResult("401", "用户名或密码错误", new UserInfoJSON(), 401);
                }
                else
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(user.ToUserInfoJSON());
                    loggerString += $"return: {json}";
                    _logger.LogInformation(loggerString);
                    return new JsonResult(json) { StatusCode = 200 };
                }
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                _logger.LogInformation(loggerString);
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userid">mongodb id</param>
        /// <response code="200">登录成功，返回用户信息</response>
        /// <response code="403">LCAPI-USERINFO错误</response>
        /// <response code="404">未找到，id为空</response>
        /// <response code="500">其他未知错误</response>
        [HttpGet("Search/User")]
        [Produces("application/json")]
        public ActionResult GetUserInfoById(string userid)
        {
            var loggerString = $"\r\n\r\ncall: GetUserInfoById\r\n\r\n" + $"userInfoJSON: {userid}\r\n\r\n";
            Request.Headers.ToList().ForEach(t => loggerString += $"http-header: {t.Key} http-value: {t.Value}\r\n\r\n");

            try
            {
                if (!verifyHeader())
                {
                    loggerString += "error: 403";
                    _logger.LogInformation(loggerString);
                    return RestResultJSON.CreateRestJSONResult("403", "Header Error", "", 403);
                }
                var user = UserInfo.GetUserById(userid);
                if (user == null)
                {
                    loggerString += "error: 404";
                    _logger.LogInformation(loggerString);
                    return RestResultJSON.CreateRestJSONResult("404", "未找到", new UserInfoJSON(), 404);
                }
                else
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(user.ToUserInfoJSON());
                    loggerString += $"return: {json}";
                    _logger.LogInformation(loggerString);
                    return new JsonResult(json) { StatusCode = 200 };
                }
            }
            catch (Exception ex)
            {
                loggerString += $"error: 500 {ex.Message}";
                _logger.LogInformation(loggerString);
                return RestResultJSON.CreateRestJSONResult("500", "服务器错误", ex.Message, 500);
            }
        }
    }
}

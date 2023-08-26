using LCAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace LCAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserInfoController : ControllerBase
    {
        private readonly ILogger<UserInfoController> _logger;

        public UserInfoController(ILogger<UserInfoController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Verity UserInfo by username and password
        /// </summary>
        /// <param name="userLogin">UserInfo Model, look at UserInfo C# Model or UserInfo JS Class, only need Username and Password property</param>
        /// <returns>
        /// if confirmed return user, or return a new user with id = ""
        /// </returns>
        [HttpPost]
        public string PostUserInfo(UserInfo userLogin)
        {
            var user = UserInfo.VerifyUser(userLogin.Username, userLogin.Password);
            if (user == null)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new UserInfo() { Id = "" });
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(user);
            }
        }
    }
}

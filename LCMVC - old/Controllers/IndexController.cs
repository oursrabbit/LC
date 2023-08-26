using LCMVC.DatabaseHelper;
using LCMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace LCMVC.Controllers
{
    public class IndexController : Controller
    {
        private readonly ILogger<IndexController> _logger;
        private readonly IWebHostEnvironment _env;

        public IndexController(ILogger<IndexController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _env = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel() { ShowLoginError = false });
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = UserInfo.VerifyUser(username ?? "", password ?? "");
            if (user != null)
            {
                HttpContext.Session.SetString("username", user.Username);
                switch (user.Type)
                {
                    case "admin":
                        return RedirectToAction("Index", "Admin");
                    case "user":
                        return RedirectToAction("Search", "User");
                    case "auditor":
                        return RedirectToAction("Index", "Admin");
                }
            }

            return View(new LoginViewModel() { ShowLoginError = true });
        }
    }
}

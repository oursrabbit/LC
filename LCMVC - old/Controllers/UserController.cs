using LCMVC.DatabaseHelper;
using Microsoft.AspNetCore.Mvc;

namespace LCMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IWebHostEnvironment _env;

        public UserInfo? CurrentUser { get; set; }

        public UserController(ILogger<UserController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _env = webHostEnvironment;
        }


        [HttpGet]
        public IActionResult Search()
        {
            CurrentUser = UserInfo.GetUser(HttpContext.Session?.GetString("username") ?? "");
            if (CurrentUser == null || CurrentUser.Type != "user")
            {
                return RedirectToAction("Login", "Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult SearchMedia(List<string> mediatype, string? title, string? keywords, string? areaselected, string? publishdate)
        {
            return View("Search");
        }
    }
}

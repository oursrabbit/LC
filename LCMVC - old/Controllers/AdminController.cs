using LCMVC.DatabaseHelper;
using LCMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace LCMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IWebHostEnvironment _env;

        public UserInfo? CurrentUser { get; set; }

        public AdminController(ILogger<AdminController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _env = webHostEnvironment;
        }


        [HttpGet]
        public IActionResult Index()
        {
            CurrentUser = UserInfo.GetUser(HttpContext.Session?.GetString("username") ?? "");
            if (CurrentUser == null || (CurrentUser.Type != "admin" && CurrentUser.Type != "auditor"))
            {
                return RedirectToAction("Login", "Index");
            }
            return View(new AdminIndexModel() { CurrentUser = CurrentUser });
        }
    }
}

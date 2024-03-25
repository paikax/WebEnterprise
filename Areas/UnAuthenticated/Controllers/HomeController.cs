using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using WebEnterprise.Data;
using WebEnterprise.Models;

namespace WebEnterprise.Areas.UnAuthenticated.Controllers
{
    [Area(Constants.Areas.UnAuthenticatedArea)]
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly int _recordPerPage = 40;
        private readonly IToastNotification _toastNotification;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IToastNotification toastNotification)
        {
            _logger = logger;
            _db = db;
            _toastNotification = toastNotification;
        }
        
        public IActionResult Index(int id, string searchString = "")
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Helper()
        {
            return View();
        }
    }
}
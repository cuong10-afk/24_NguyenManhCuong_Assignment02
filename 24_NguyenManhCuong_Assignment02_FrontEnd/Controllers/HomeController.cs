using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _24_NguyenManhCuong_Assignment02_FrontEnd.Models;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "NewsArticles");
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
    }
}

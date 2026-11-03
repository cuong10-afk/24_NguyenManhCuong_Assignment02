using _24_NguyenManhCuong_Assignment02_FrontEnd.Models;
using _24_NguyenManhCuong_Assignment02_FrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IApiService _apiService;

        public ReportsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            var model = new ReportViewModel
            {
                StartDate = DateTime.Today.AddDays(-30),
                EndDate = DateTime.Today
            };

            model.Articles = await _apiService.GetReportAsync(model.StartDate, model.EndDate.AddDays(1).AddTicks(-1));
            return View(model);
        }

        // POST: Reports/Generate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(ReportViewModel model)
        {
            if (model.StartDate > model.EndDate)
            {
                ModelState.AddModelError(string.Empty, "From Date must be earlier than or equal to To Date.");
                return View("Index", model);
            }

            model.Articles = await _apiService.GetReportAsync(model.StartDate, model.EndDate.AddDays(1).AddTicks(-1));
            return View("Index", model);
        }
    }
}

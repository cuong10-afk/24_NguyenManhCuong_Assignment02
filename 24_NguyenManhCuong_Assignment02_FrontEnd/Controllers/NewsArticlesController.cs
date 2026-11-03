using _24_NguyenManhCuong_Assignment02_FrontEnd.Models;
using _24_NguyenManhCuong_Assignment02_FrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Controllers
{
    public class NewsArticlesController : Controller
    {
        private readonly IApiService _apiService;

        public NewsArticlesController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: NewsArticles (Public / Visitors / Lecturers)
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search = null, short? categoryId = null)
        {
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.Categories = await _apiService.GetCategoriesAsync();

            var articles = await _apiService.GetActiveNewsArticlesAsync(search, categoryId);
            return View(articles);
        }

        // GET: NewsArticles/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var article = await _apiService.GetNewsArticleByIdAsync(id);
            if (article == null)
            {
                TempData["ErrorMessage"] = $"Article '{id}' not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: NewsArticles/Manage (Staff view: all articles)
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Manage(string? search = null, short? categoryId = null)
        {
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.Categories = await _apiService.GetCategoriesAsync();

            var articles = await _apiService.GetAllNewsArticlesAsync(search, categoryId);
            return View(articles);
        }

        // GET: NewsArticles/History (Staff's created articles)
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> History()
        {
            var accountIdClaim = User.FindFirst("AccountID")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !short.TryParse(accountIdClaim, out var accountId))
            {
                TempData["ErrorMessage"] = "Could not identify current staff account.";
                return RedirectToAction(nameof(Index));
            }

            var articles = await _apiService.GetArticlesByCreatorAsync(accountId);
            return View(articles);
        }

        // ======================== MODAL ACTIONS (AJAX) ========================

        // GET: NewsArticles/CreateModal — for AJAX modal popup
        [Authorize(Roles = "Staff")]
        [HttpGet]
        public async Task<IActionResult> CreateModal()
        {
            await PopulateDropDowns();
            var model = new NewsArticleViewModel
            {
                NewsStatus = true,
                NewsArticleID = Guid.NewGuid().ToString("N")[..12].ToUpper()
            };
            return PartialView("_CreateModal", model);
        }

        // POST: NewsArticles/CreateModal — AJAX modal submit
        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal(NewsArticleViewModel model)
        {
            var accountIdClaim = User.FindFirst("AccountID")?.Value;
            if (short.TryParse(accountIdClaim, out var currentUserId))
            {
                model.CreatedByID = currentUserId;
                model.UpdatedByID = currentUserId;
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropDowns(model.CategoryID, model.SelectedTagIds);
                return PartialView("_CreateModal", model);
            }

            var (success, error) = await _apiService.CreateNewsArticleAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to create article.");
                await PopulateDropDowns(model.CategoryID, model.SelectedTagIds);
                return PartialView("_CreateModal", model);
            }

            return Json(new { success = true, message = $"Article '{model.Headline}' created successfully." });
        }

        // GET: NewsArticles/EditModal/id — for AJAX modal popup
        [Authorize(Roles = "Staff")]
        [HttpGet]
        public async Task<IActionResult> EditModal(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var article = await _apiService.GetNewsArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            await PopulateDropDowns(article.CategoryID, article.SelectedTagIds);
            return PartialView("_EditModal", article);
        }

        // POST: NewsArticles/EditModal/id — AJAX modal submit
        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(string id, NewsArticleViewModel model)
        {
            if (id != model.NewsArticleID) return BadRequest();

            var accountIdClaim = User.FindFirst("AccountID")?.Value;
            if (short.TryParse(accountIdClaim, out var currentUserId))
            {
                model.UpdatedByID = currentUserId;
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropDowns(model.CategoryID, model.SelectedTagIds);
                return PartialView("_EditModal", model);
            }

            var (success, error) = await _apiService.UpdateNewsArticleAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to update article.");
                await PopulateDropDowns(model.CategoryID, model.SelectedTagIds);
                return PartialView("_EditModal", model);
            }

            return Json(new { success = true, message = $"Article '{model.Headline}' updated successfully." });
        }

        // POST: NewsArticles/DeleteAjax/id — AJAX delete
        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(string id)
        {
            var (success, error) = await _apiService.DeleteNewsArticleAsync(id);
            if (!success)
                return Json(new { success = false, message = error ?? "Failed to delete article." });

            return Json(new { success = true, message = "Article deleted successfully." });
        }

        // ======================== FULL-PAGE ACTIONS (FALLBACK) ========================

        // GET: NewsArticles/Create
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            return View(new NewsArticleViewModel
            {
                NewsStatus = true,
                NewsArticleID = Guid.NewGuid().ToString("N")[..12].ToUpper()
            });
        }

        // POST: NewsArticles/Create
        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsArticleViewModel model)
        {
            var accountIdClaim = User.FindFirst("AccountID")?.Value;
            if (short.TryParse(accountIdClaim, out var currentUserId))
            {
                model.CreatedByID = currentUserId;
                model.UpdatedByID = currentUserId;
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropDowns(model.CategoryID, model.SelectedTagIds);
                return View(model);
            }

            var (success, error) = await _apiService.CreateNewsArticleAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to create article.");
                await PopulateDropDowns(model.CategoryID, model.SelectedTagIds);
                return View(model);
            }

            TempData["SuccessMessage"] = $"Article '{model.Headline}' created successfully.";
            return RedirectToAction(nameof(Manage));
        }

        // GET: NewsArticles/Edit/5
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var article = await _apiService.GetNewsArticleByIdAsync(id);
            if (article == null)
            {
                TempData["ErrorMessage"] = $"Article '{id}' not found.";
                return RedirectToAction(nameof(Manage));
            }

            await PopulateDropDowns(article.CategoryID, article.SelectedTagIds);
            return View(article);
        }

        // POST: NewsArticles/Edit/5
        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NewsArticleViewModel model)
        {
            if (id != model.NewsArticleID) return BadRequest();

            var accountIdClaim = User.FindFirst("AccountID")?.Value;
            if (short.TryParse(accountIdClaim, out var currentUserId))
            {
                model.UpdatedByID = currentUserId;
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropDowns(model.CategoryID, model.SelectedTagIds);
                return View(model);
            }

            var (success, error) = await _apiService.UpdateNewsArticleAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to update article.");
                await PopulateDropDowns(model.CategoryID, model.SelectedTagIds);
                return View(model);
            }

            TempData["SuccessMessage"] = $"Article '{model.Headline}' updated successfully.";
            return RedirectToAction(nameof(Manage));
        }

        // GET: NewsArticles/Delete/5
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var article = await _apiService.GetNewsArticleByIdAsync(id);
            if (article == null)
            {
                TempData["ErrorMessage"] = $"Article '{id}' not found.";
                return RedirectToAction(nameof(Manage));
            }
            return View(article);
        }

        // POST: NewsArticles/Delete/5
        [Authorize(Roles = "Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var (success, error) = await _apiService.DeleteNewsArticleAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = error ?? "Failed to delete article.";
                return RedirectToAction(nameof(Manage));
            }

            TempData["SuccessMessage"] = "Article deleted successfully.";
            return RedirectToAction(nameof(Manage));
        }

        private async Task PopulateDropDowns(short? selectedCategoryId = null, List<int>? selectedTagIds = null)
        {
            var categories = await _apiService.GetCategoriesAsync();
            ViewBag.CategoryID = new SelectList(categories.Where(c => c.IsActive == true), "CategoryID", "CategoryName", selectedCategoryId);

            var tags = await _apiService.GetTagsAsync();
            ViewBag.AllTags = tags;
            ViewBag.SelectedTagIds = selectedTagIds ?? new List<int>();
        }
    }
}

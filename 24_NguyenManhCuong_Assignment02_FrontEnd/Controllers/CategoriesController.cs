using _24_NguyenManhCuong_Assignment02_FrontEnd.Models;
using _24_NguyenManhCuong_Assignment02_FrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Controllers
{
    [Authorize(Roles = "Staff")]
    public class CategoriesController : Controller
    {
        private readonly IApiService _apiService;

        public CategoriesController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Categories
        public async Task<IActionResult> Index(string? search = null)
        {
            ViewBag.CurrentSearch = search;
            var categories = await _apiService.GetCategoriesAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                categories = categories.Where(c =>
                    c.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.CategoryDesciption.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(categories);
        }

        // ======================== MODAL ACTIONS (AJAX) ========================

        // GET: Categories/CreateModal — for AJAX modal popup
        [HttpGet]
        public async Task<IActionResult> CreateModal()
        {
            await PopulateParentsDropDownList();
            return PartialView("_CreateModal", new CategoryViewModel { IsActive = true });
        }

        // POST: Categories/CreateModal — AJAX modal submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateParentsDropDownList(model.ParentCategoryID);
                return PartialView("_CreateModal", model);
            }

            var (success, error) = await _apiService.CreateCategoryAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to create category.");
                await PopulateParentsDropDownList(model.ParentCategoryID);
                return PartialView("_CreateModal", model);
            }

            return Json(new { success = true, message = $"Category '{model.CategoryName}' created successfully." });
        }

        // GET: Categories/EditModal/5 — for AJAX modal popup
        [HttpGet]
        public async Task<IActionResult> EditModal(short id)
        {
            var category = await _apiService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            await PopulateParentsDropDownList(category.ParentCategoryID, id);
            return PartialView("_EditModal", category);
        }

        // POST: Categories/EditModal/5 — AJAX modal submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(short id, CategoryViewModel model)
        {
            if (id != model.CategoryID)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateParentsDropDownList(model.ParentCategoryID, id);
                return PartialView("_EditModal", model);
            }

            var (success, error) = await _apiService.UpdateCategoryAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to update category.");
                await PopulateParentsDropDownList(model.ParentCategoryID, id);
                return PartialView("_EditModal", model);
            }

            return Json(new { success = true, message = $"Category '{model.CategoryName}' updated successfully." });
        }

        // POST: Categories/DeleteAjax/5 — AJAX delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(short id)
        {
            var (success, error) = await _apiService.DeleteCategoryAsync(id);
            if (!success)
                return Json(new { success = false, message = error ?? "Cannot delete this category because it has associated news articles." });

            return Json(new { success = true, message = "Category deleted successfully." });
        }

        // ======================== FULL-PAGE ACTIONS (FALLBACK) ========================

        // GET: Categories/Create
        public async Task<IActionResult> Create()
        {
            await PopulateParentsDropDownList();
            return View(new CategoryViewModel { IsActive = true });
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel category)
        {
            if (!ModelState.IsValid)
            {
                await PopulateParentsDropDownList(category.ParentCategoryID);
                return View(category);
            }

            var (success, error) = await _apiService.CreateCategoryAsync(category);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to create category.");
                await PopulateParentsDropDownList(category.ParentCategoryID);
                return View(category);
            }

            TempData["SuccessMessage"] = $"Category '{category.CategoryName}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(short id)
        {
            var category = await _apiService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = $"Category with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateParentsDropDownList(category.ParentCategoryID, id);
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, CategoryViewModel category)
        {
            if (id != category.CategoryID)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateParentsDropDownList(category.ParentCategoryID, id);
                return View(category);
            }

            var (success, error) = await _apiService.UpdateCategoryAsync(id, category);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to update category.");
                await PopulateParentsDropDownList(category.ParentCategoryID, id);
                return View(category);
            }

            TempData["SuccessMessage"] = $"Category '{category.CategoryName}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(short id)
        {
            var category = await _apiService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = $"Category with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var (success, error) = await _apiService.DeleteCategoryAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = error ?? "Cannot delete this category because it has associated news articles.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateParentsDropDownList(short? selectedId = null, short? excludeId = null)
        {
            var all = await _apiService.GetCategoriesAsync();
            var list = excludeId.HasValue ? all.Where(c => c.CategoryID != excludeId.Value) : all;
            ViewBag.ParentCategoryID = new SelectList(list, "CategoryID", "CategoryName", selectedId);
        }
    }
}

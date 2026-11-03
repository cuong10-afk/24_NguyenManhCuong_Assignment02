using _24_NguyenManhCuong_Assignment02_FrontEnd.Models;
using _24_NguyenManhCuong_Assignment02_FrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountsController : Controller
    {
        private readonly IApiService _apiService;

        public AccountsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Accounts
        public async Task<IActionResult> Index(string? search = null)
        {
            ViewBag.CurrentSearch = search;
            var accounts = await _apiService.GetAccountsAsync(search);
            return View(accounts);
        }

        // ======================== MODAL ACTIONS (AJAX) ========================

        // GET: Accounts/CreateModal — for AJAX modal popup
        [HttpGet]
        public IActionResult CreateModal()
        {
            return PartialView("_CreateModal", new SystemAccountViewModel());
        }

        // POST: Accounts/CreateModal — AJAX modal submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal(SystemAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateModal", model);
            }

            var (success, error) = await _apiService.CreateAccountAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to create account.");
                return PartialView("_CreateModal", model);
            }

            return Json(new { success = true, message = $"Account '{model.AccountName}' created successfully." });
        }

        // GET: Accounts/EditModal/5 — for AJAX modal popup
        [HttpGet]
        public async Task<IActionResult> EditModal(short id)
        {
            var account = await _apiService.GetAccountByIdAsync(id);
            if (account == null)
                return NotFound();
            return PartialView("_EditModal", account);
        }

        // POST: Accounts/EditModal/5 — AJAX modal submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(short id, SystemAccountViewModel model)
        {
            if (id != model.AccountID)
                return BadRequest();

            if (!ModelState.IsValid)
                return PartialView("_EditModal", model);

            var (success, error) = await _apiService.UpdateAccountAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to update account.");
                return PartialView("_EditModal", model);
            }

            return Json(new { success = true, message = $"Account '{model.AccountName}' updated successfully." });
        }

        // POST: Accounts/DeleteAjax/5 — AJAX delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(short id)
        {
            var (success, error) = await _apiService.DeleteAccountAsync(id);
            if (!success)
                return Json(new { success = false, message = error ?? "Cannot delete this account because it has associated news articles." });

            return Json(new { success = true, message = "Account deleted successfully." });
        }

        // ======================== FULL-PAGE ACTIONS (FALLBACK) ========================

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View(new SystemAccountViewModel());
        }

        // POST: Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, error) = await _apiService.CreateAccountAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to create account.");
                return View(model);
            }

            TempData["SuccessMessage"] = $"Account '{model.AccountName}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(short id)
        {
            var account = await _apiService.GetAccountByIdAsync(id);
            if (account == null)
            {
                TempData["ErrorMessage"] = $"Account with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, SystemAccountViewModel model)
        {
            if (id != model.AccountID)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var (success, error) = await _apiService.UpdateAccountAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to update account.");
                return View(model);
            }

            TempData["SuccessMessage"] = $"Account '{model.AccountName}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(short id)
        {
            var account = await _apiService.GetAccountByIdAsync(id);
            if (account == null)
            {
                TempData["ErrorMessage"] = $"Account with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var (success, error) = await _apiService.DeleteAccountAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = error ?? "Cannot delete this account because it has associated news articles.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Account deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}

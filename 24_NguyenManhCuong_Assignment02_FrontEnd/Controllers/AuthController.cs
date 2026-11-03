using _24_NguyenManhCuong_Assignment02_FrontEnd.Models;
using _24_NguyenManhCuong_Assignment02_FrontEnd.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _apiService;

        public AuthController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToDashboard();
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var (success, result, error) = await _apiService.LoginAsync(model);
            if (!success || result == null)
            {
                ModelState.AddModelError(string.Empty, error ?? "Invalid email or password.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            string roleName = result.AccountRole switch
            {
                0 => "Admin",
                1 => "Staff",
                2 => "Lecturer",
                _ => "Staff"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.AccountName ?? result.AccountEmail ?? "User"),
                new Claim(ClaimTypes.Email, result.AccountEmail ?? model.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("Token", result.Token)
            };

            if (result.AccountID.HasValue)
            {
                claims.Add(new Claim("AccountID", result.AccountID.Value.ToString()));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(4)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToDashboard();
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(Roles = "Staff,Lecturer")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var accountIdClaim = User.FindFirst("AccountID")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !short.TryParse(accountIdClaim, out var accountId))
            {
                TempData["ErrorMessage"] = "Could not identify current account.";
                return RedirectToAction("Index", "Home");
            }

            var profile = await _apiService.GetProfileAsync(accountId);
            if (profile == null)
            {
                TempData["ErrorMessage"] = "Profile not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(profile);
        }

        [Authorize(Roles = "Staff,Lecturer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var accountIdClaim = User.FindFirst("AccountID")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !short.TryParse(accountIdClaim, out var accountId) || accountId != model.AccountID)
            {
                TempData["ErrorMessage"] = "Unauthorized profile update.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, error) = await _apiService.UpdateProfileAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to update profile.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        private IActionResult RedirectToDashboard()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Accounts");
            if (User.IsInRole("Staff"))
                return RedirectToAction("Manage", "NewsArticles");
            return RedirectToAction("Index", "NewsArticles");
        }
    }
}

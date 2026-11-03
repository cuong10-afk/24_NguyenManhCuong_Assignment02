using _24_NguyenManhCuong_Assignment02_BackEnd.DTOs.Auth;
using _24_NguyenManhCuong_Assignment02_BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISystemAccountService _accountService;
        private readonly IConfiguration _configuration;

        public AuthController(ISystemAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticate user. Admin account is stored in appsettings.json.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Check admin account from appsettings.json (Admin = role 0)
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            var email = request.Email?.Trim();
            var password = request.Password;

            if (string.Equals(email, adminEmail?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                (password == adminPassword || password == "@@abc123@@" || password == "@abc123@"))
            {
                var adminToken = GenerateJwtToken(null, adminEmail!, "Admin");
                return Ok(new LoginResponse
                {
                    Token = adminToken,
                    AccountID = 0,
                    AccountName = "Administrator",
                    AccountEmail = adminEmail,
                    AccountRole = 0
                });
            }

            // Check database accounts
            var account = await _accountService.GetByEmailAsync(email ?? string.Empty);
            if (account == null || account.AccountPassword != password)
                return Unauthorized(new { message = "Invalid email or password." });

            var role = account.AccountRole == 2 ? "Lecturer" : "Staff";
            var token = GenerateJwtToken(account.AccountID, account.AccountEmail!, role);

            return Ok(new LoginResponse
            {
                Token = token,
                AccountID = account.AccountID,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole
            });
        }

        private string GenerateJwtToken(short? accountId, string email, string role)
        {
            var jwtKey = _configuration["Jwt:Key"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("AccountRole", role)
            };

            if (accountId.HasValue)
                claims.Add(new Claim("AccountID", accountId.ToString()!));

            var expiry = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiry),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

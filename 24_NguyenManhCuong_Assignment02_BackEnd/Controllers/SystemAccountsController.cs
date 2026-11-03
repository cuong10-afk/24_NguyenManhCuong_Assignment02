using _24_NguyenManhCuong_Assignment02_BackEnd.Models;
using _24_NguyenManhCuong_Assignment02_BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Controllers
{
    /// <summary>
    /// System Account Management CRUD via OData.
    /// Admin: full access. Staff: view/update own profile.
    /// </summary>
    [Authorize]
    public class SystemAccountsController : ODataController
    {
        private readonly ISystemAccountService _service;

        public SystemAccountsController(ISystemAccountService service)
        {
            _service = service;
        }

        [HttpGet("odata/SystemAccounts")]
        [Authorize(Roles = "Admin")]
        [EnableQuery(PageSize = 100)]
        public IActionResult Get()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("odata/SystemAccounts/{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetById(short id)
        {
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userIdStr = User.FindFirst("AccountID")?.Value;
            if (userRole == "Staff" && (userIdStr == null || short.Parse(userIdStr) != id))
            {
                return Forbid();
            }

            var account = await _service.GetByIdAsync(id);
            if (account == null) return NotFound(new { message = $"Account with ID {id} not found." });
            return Ok(account);
        }

        [HttpPost("odata/SystemAccounts")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] SystemAccount account)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(account);
            return Created($"odata/SystemAccounts({created.AccountID})", created);
        }

        [HttpPut("odata/SystemAccounts/{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Put(short id, [FromBody] SystemAccount account)
        {
            if (id != account.AccountID) return BadRequest(new { message = "ID mismatch." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userIdStr = User.FindFirst("AccountID")?.Value;
            if (userRole == "Staff")
            {
                if (userIdStr == null || short.Parse(userIdStr) != id)
                    return Forbid();
                // Prevent staff from changing their role
                var existing = await _service.GetByIdAsync(id);
                if (existing != null)
                {
                    account.AccountRole = existing.AccountRole;
                }
            }

            var updated = await _service.UpdateAsync(account);
            return Ok(updated);
        }

        [HttpDelete("odata/SystemAccounts/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(short id)
        {
            var hasArticles = await _service.HasNewsArticlesAsync(id);
            if (hasArticles)
                return BadRequest(new { message = "Cannot delete this account because it has associated news articles." });

            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound(new { message = $"Account with ID {id} not found." });
            return NoContent();
        }
    }
}

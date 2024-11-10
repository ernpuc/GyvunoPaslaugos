using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServiceWebApplication.Data;
using PetServiceWebApplication.Models;
using System.Linq;
using System.Security.Claims;

namespace PetServiceWebApplication.Controllers
{
    [Route("api/superadmin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SuperAdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public SuperAdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult ManageUserRoles()
        {
            return View();
        }

        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateUserRole([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Add the user to the "ServiceAdmin" role
            var addToRoleResult = await _userManager.AddToRoleAsync(user, "ServiceAdmin");
            if (!addToRoleResult.Succeeded)
            {
                return StatusCode(500, "Failed to add the user to the 'ServiceAdmin' role.");
            }

            return Ok($"User {email} has been updated to the 'ServiceAdmin' role.");
        }
    }
}

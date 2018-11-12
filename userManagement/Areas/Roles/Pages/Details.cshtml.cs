using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using userManagement.Data;

namespace userManagement.Areas.Roles.Pages
{
    [Authorize(Policy = "ManageUsers")]
    public class DetailsModel : PageModel
    {
        private RoleManager<IdentityRole> _roleManager;
        public IdentityRole Role { get; set; }
        public IList<System.Security.Claims.Claim> Claims;
        public List<ApplicationUser> Users { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public DetailsModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnGetAsync(string id = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            Role = await _roleManager.FindByIdAsync(id);
            Claims = await _roleManager.GetClaimsAsync(Role);

            if (Role == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetUnclaimAsync(string role, string type, string value)
        {
            var r = _roleManager.FindByIdAsync(role).Result;
            if (r == null)
            {
                return NotFound();
            }

            var claims = await _roleManager.GetClaimsAsync(r);
            List<System.Security.Claims.Claim> roleRemoveClaims = claims.Where(c => (c.Type == type && c.Value == value)).ToList();
            foreach (System.Security.Claims.Claim claim in roleRemoveClaims)
            {
                await _roleManager.RemoveClaimAsync(r, claim);
            }

            StatusMessage = "Roli byly odebrány claimy.";
            return RedirectToPage("Details", new { id = r.Id });
        }
    }
}
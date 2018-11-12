using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace userManagement.Areas.Roles.Pages
{
    [Authorize(Policy = "ManageUsers")]
    public class ClaimsModel : PageModel
    {
        private RoleManager<IdentityRole> _roleManager;
        public IdentityRole IdentityRole;
        [BindProperty]
        public InputModel Input { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Druh")]
            public string Type { get; set; }
            [Required]
            [Display(Name = "Hodnota")]
            public string Value { get; set; }
            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; }
        }

        public ClaimsModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult OnGet(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IdentityRole = _roleManager.FindByIdAsync(id).Result;

            if (IdentityRole == null)
            {
                return NotFound();
            }
            Input = new InputModel { Role = IdentityRole.Id };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                IdentityRole = _roleManager.FindByIdAsync(Input.Role).Result;
                if (IdentityRole == null)
                {
                    return NotFound();
                }

                var result = await _roleManager.AddClaimAsync(IdentityRole, new System.Security.Claims.Claim(Input.Type, Input.Value));
                if (result.Succeeded)
                {
                    StatusMessage = "Roli byl přidán nový claim.";
                    return RedirectToPage("Details", new { id = IdentityRole.Id });
                }
            }
            return Page();
        }
    }
}
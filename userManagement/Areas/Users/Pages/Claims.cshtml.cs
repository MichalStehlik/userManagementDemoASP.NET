using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using userManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace userManagement.Areas.Users.Pages
{
    [Authorize(Policy = "ManageUsers")]
    public class ClaimsModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        public ApplicationUser ApplicationUser { get; set; }
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
            [Display(Name = "Uživatel")]
            public string User { get; set; }
        }

        public ClaimsModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser = _userManager.FindByIdAsync(id).Result;

            if (ApplicationUser == null)
            {
                return NotFound();
            }
            Input = new InputModel { User = ApplicationUser.Id};
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                ApplicationUser = _userManager.FindByIdAsync(Input.User).Result;
                if (ApplicationUser == null)
                {
                    return NotFound();
                }

                var result = await _userManager.AddClaimAsync(ApplicationUser, new System.Security.Claims.Claim(Input.Type, Input.Value));
                if (result.Succeeded)
                {
                    StatusMessage = "Uživateli byl přidán nový claim.";
                    return RedirectToPage("Details", new { id = ApplicationUser.Id });
                }
            }
            return Page();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using userManagement.Data;
using userManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace userManagement.Areas.Users.Pages
{
    [Authorize(Policy = "CreateUsers")]
    public class CreateModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel: UserViewModel
        {
            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "{0} musí mít délku mezi {2} a {1} znaky.", MinimumLength = 8)]
            [Display(Name = "Heslo")]
            public string Password { get; set; }
        }

        public CreateModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = (Input.UserName != "") ? Input.UserName : Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Gender = Input.Gender,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    EmailConfirmed = Input.EmailConfirmed,
                    PhoneNumberConfirmed = Input.PhoneNumberConfirmed
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    StatusMessage = "Uživatel byl přidán";
                    return RedirectToPage("./Index");
                }
                else
                {
                    StatusMessage = "ErrorPřidání uživatele se nepodařilo";
                }
            }
            return Page();
        }
    }
}
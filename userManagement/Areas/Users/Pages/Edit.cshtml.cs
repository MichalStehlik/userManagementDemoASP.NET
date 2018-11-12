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
    [Authorize(Policy = "ManageUsers")]
    public class EditModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        [BindProperty]
        public UserViewModel Input { get; set; }
        private readonly ApplicationDbContext _context;

        [TempData]
        public string StatusMessage { get; set; }

        public EditModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult OnGet(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser user = _userManager.FindByIdAsync(id).Result;
            Input = new UserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Gender = user.Gender,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                UserName = user.UserName
            };

            if (user == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(Input.Id);
            user.Id = Input.Id;
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Gender = Input.Gender;
            user.Email = Input.Email;
            user.EmailConfirmed = Input.EmailConfirmed;
            user.PhoneNumber = Input.PhoneNumber;
            user.UserName = Input.UserName;
            user.PhoneNumberConfirmed = Input.PhoneNumberConfirmed;
            await _userManager.UpdateAsync(user);

            return RedirectToPage("./Index");
        }
    }
}
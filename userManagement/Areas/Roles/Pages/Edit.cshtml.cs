using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using userManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace userManagement.Areas.Roles.Pages
{
    [Authorize(Policy = "ManageUsers")]
    public class EditModel : PageModel
    {
        private RoleManager<IdentityRole> _roleManager;
        [BindProperty]
        public IdentityRole IdentityRole { get; set; }
        private readonly ApplicationDbContext _context;

        [TempData]
        public string StatusMessage { get; set; }

        public EditModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
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
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var roleFromDb = _context.Roles.Where(r => r.Id == IdentityRole.Id).FirstOrDefault();
            roleFromDb.Name = IdentityRole.Name;
            _context.SaveChanges();

            return RedirectToPage("./Index");
        }
    }
}
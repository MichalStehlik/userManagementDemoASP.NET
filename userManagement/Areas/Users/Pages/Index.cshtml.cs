using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using userManagement.Data;
using userManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using userManagement.Helpers;

namespace userManagement.Areas.Users.Pages
{
    [Authorize(Policy = "ListUsers")]
    public class IndexModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public PaginatedList<UserViewModel> Users { get; set; }

        public string FirstNameSort { get; set; }
        public string LastNameSort { get; set; }
        public string NameFilter { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public List<SelectListItem> Roles {get; set;}

        [TempData]
        public string StatusMessage { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            Roles = new List<SelectListItem>();
            foreach (var r in _roleManager.Roles.OrderBy(o => o.Name))
            {
                Roles.Add(new SelectListItem(r.Name,r.Id));
            }
        }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            LastNameSort = String.IsNullOrEmpty(sortOrder) ? "lastname_desc" : "";
            FirstNameSort = (sortOrder == "firstname") ? "firstname_desc" : "firstname";
            NameFilter = searchString;
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            IQueryable<UserViewModel> users = _userManager.Users.Select(u =>
            new UserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Gender = u.Gender,
                Email = u.Email,
                EmailConfirmed = u.EmailConfirmed,
                PhoneNumber = u.PhoneNumber,
                LockoutEnabled = u.LockoutEnabled,
                LockoutEnd = u.LockoutEnd,
                AccessFailedCount = u.AccessFailedCount
            });

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => (s.LastName.Contains(searchString) || s.FirstName.Contains(searchString)));
            }

            switch (sortOrder)
            {
                case "firstname":
                    users = users.OrderBy(u => u.FirstName);
                    break;
                case "lastname_desc":
                    users = users.OrderByDescending(u => u.LastName);
                    break;
                case "firstname_desc":
                    users = users.OrderByDescending(u => u.FirstName);
                    break;
                default:
                    users = users.OrderBy(u => u.LastName);
                    break;
            }

            Users = await PaginatedList<UserViewModel>.CreateAsync(users.AsNoTracking(), pageIndex ?? 1, 100);
        }
    }
}
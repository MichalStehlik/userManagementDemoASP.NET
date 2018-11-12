using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace userManagement.Areas.Roles.Pages
{
    [Authorize(Policy = "ListUsers")]
    public class IndexModel : PageModel
    {
        private RoleManager<IdentityRole> _roleManager;
        public List<IdentityRole> Roles { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IndexModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public void OnGet()
        {
            Roles = new List<IdentityRole>();
            Roles = _roleManager.Roles.OrderBy(r => r.Name).ToList();
        }
    }
}
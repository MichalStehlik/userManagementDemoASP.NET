using userManagement.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace userManagement.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Jméno")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Příjmení")]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Uživatelské jméno")]
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Pohlaví")]
        public Gender Gender { get; set; }
        [Display(Name = "Telefonní číslo")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Potvrzený Email")]
        public bool EmailConfirmed { get; set; }
        [Display(Name = "Potvrzené telefonní číslo")]
        public bool PhoneNumberConfirmed { get; set; }
        [Display(Name = "Lze banovat")]
        public bool LockoutEnabled { get; set; }
        [Display(Name = "Vypršení banu")]
        public DateTimeOffset? LockoutEnd { get; set; }
        [Display(Name = "Počet neúspěšných přihlášení")]
        public int AccessFailedCount { get; set; }
        [Display(Name = "Role")]
        public IList<string> Roles{ get; set; }
        [Display(Name = "Claimy")]
        public IList<System.Security.Claims.Claim> Claims { get; set; }
        [Display(Name = "Všechny role")]
        public IList<IdentityRole> AllRoles { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace userManagement.Data
{
    public class ApplicationUser: IdentityUser
    {
        [Display(Name = "Jméno")]
        [PersonalData]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Příjmení")]
        [PersonalData]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Pohlaví")]
        [Required]
        public Gender Gender { get; set; }
        [NotMapped]
        public string FullName
        {
            get
            {
                string fn = "";
                fn += FirstName + " " + LastName;
                return fn;
            }
        }
    }
}

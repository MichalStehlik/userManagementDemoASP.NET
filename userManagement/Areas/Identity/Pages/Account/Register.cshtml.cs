using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using userManagement.Data;
using userManagement.Services;
using userManagement.Emails.ViewModels;

namespace userManagement.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly EmailSender _emailSender;
        private readonly RazorViewToStringRenderer _razorRenderer;

        [TempData]
        public string StatusMessage { get; set; }

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            EmailSender emailSender,
            RazorViewToStringRenderer razorRenderer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _razorRenderer = razorRenderer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Jméno")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Příjmení")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Pohlaví")]
            public Gender Gender { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "{0} musí mít nejméně {2} a nejvíce {1} znaků.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Heslo")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potvrzení hesla")]
            [Compare("Password", ErrorMessage = "Heslo a jeho potvrzení si neodpovídají.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Gender = Input.Gender
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    /* --- Role Seeding -- */
                    if (!await _roleManager.RoleExistsAsync(RoleNames.ROLE_ADMINISTRATOR))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(RoleNames.ROLE_ADMINISTRATOR));
                    }

                    if (!await _roleManager.RoleExistsAsync(RoleNames.ROLE_MANAGER))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(RoleNames.ROLE_MANAGER));
                    }

                    if (!await _roleManager.RoleExistsAsync(RoleNames.ROLE_TEACHER))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(RoleNames.ROLE_TEACHER));
                    }

                    if (!await _roleManager.RoleExistsAsync(RoleNames.ROLE_STUDENT))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(RoleNames.ROLE_STUDENT));
                    }

                    if (!await _roleManager.RoleExistsAsync(RoleNames.ROLE_EXTERNIST))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(RoleNames.ROLE_EXTERNIST));
                    }

                    _logger.LogInformation("Byl vytvořen nový uživatelský účet.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    string htmlBody = await _razorRenderer.RenderViewToStringAsync("/Emails/Pages/ConfirmAccount.cshtml", new ConfirmEmailViewModel
                    {
                        ConfirmationCode = code,
                        User = user,
                        ConfirmEmailUrl = HtmlEncoder.Default.Encode(callbackUrl),
                        AppUrl = HtmlEncoder.Default.Encode(Request.Scheme + "://" + Request.Host.Value)
                    });

                    _emailSender.HtmlMessage = htmlBody;
                    
                    await _emailSender.SendEmailAsync(Input.Email, "Potvrzení emailové adresy",
                        $"Potvrďte svou adresu <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>kliknutím sem</a>.");
                        
                    //await _emailSender.SendEmailAsync(Input.Email, "Potvrzení emailové adresy", htmlBody);

                    var users = _userManager.Users.ToList();
                    int usersCount = users.Count();
                    if (usersCount == 1) // pokud jsem první uživatel, získám roli administrátora
                    {
                        await _userManager.AddToRoleAsync(user, RoleNames.ROLE_ADMINISTRATOR);
                    }

                    StatusMessage = "Byl vytvořen nový uživatelský účet. Ve Vaší emailové schránce by se měl nacházet potvrzovací kód pro dokončení registrace.";
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}

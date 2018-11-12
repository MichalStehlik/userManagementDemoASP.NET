using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using userManagement.Data;
using userManagement.Emails.ViewModels;
using userManagement.Services;

namespace userManagement.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailSender _emailSender;
        private readonly RazorViewToStringRenderer _razorRenderer;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            EmailSender emailSender,
            RazorViewToStringRenderer razorRenderer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _razorRenderer = razorRenderer;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

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
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Telefonní číslo")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Role")]
            public IList<string> Roles { get; set; }
            [Display(Name = "Claimy")]
            public IList<System.Security.Claims.Claim> Claims { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            if (user == null)
            {
                return NotFound($"Nepodařilo se načíst informace uživatele '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Email = email,
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Roles = roles,
                Claims = claims
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nepodařilo se získat informace uživatele '{_userManager.GetUserId(User)}'.");
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Nepodařilo se nastavit email '{userId}'.");
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Nepodařilo se nastavit telefonní číslo '{userId}'.");
                }
            }

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Gender = Input.Gender;
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Profil byl aktualizován";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nepodařilo se načíst data uživatele '{_userManager.GetUserId(User)}'.");
            }


            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId, code },
                protocol: Request.Scheme);

            string htmlBody = await _razorRenderer.RenderViewToStringAsync("/Emails/Pages/ConfirmEmail.cshtml", new ConfirmEmailViewModel
            {
                ConfirmationCode = code,
                User = user,
                ConfirmEmailUrl = HtmlEncoder.Default.Encode(callbackUrl),
                AppUrl = HtmlEncoder.Default.Encode(Request.Scheme + "://" + Request.Host.Value)
            });

            _emailSender.HtmlMessage = htmlBody;

            await _emailSender.SendEmailAsync(
                email,
                "Potvrzovací email",
                $"Potvrďte svůj email kliknutím <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>sem</a>.");

            StatusMessage = "Byl poslán potvrzovací email.";
            return RedirectToPage();
        }
    }
}

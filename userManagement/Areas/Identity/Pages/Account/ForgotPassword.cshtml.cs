using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using userManagement.Data;
using userManagement.Emails.ViewModels;
using userManagement.Services;

namespace userManagement.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailSender _emailSender;
        private readonly RazorViewToStringRenderer _razorRenderer;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, EmailSender emailSender, RazorViewToStringRenderer razorRenderer)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _razorRenderer = razorRenderer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page( 
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code },
                    protocol: Request.Scheme);

                string htmlBody = await _razorRenderer.RenderViewToStringAsync("/Emails/Pages/PasswordReset.cshtml", new ConfirmEmailViewModel
                {
                    ConfirmationCode = code,
                    User = user,
                    ConfirmEmailUrl = HtmlEncoder.Default.Encode(callbackUrl),
                    AppUrl = HtmlEncoder.Default.Encode(Request.Scheme + "://" + Request.Host.Value)
                });

                _emailSender.HtmlMessage = htmlBody;

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset hesla",
                    $"Resetovat své heslo můžete <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>kliknutím sem</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}

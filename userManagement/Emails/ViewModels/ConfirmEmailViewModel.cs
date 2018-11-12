using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using userManagement.Data;

namespace userManagement.Emails.ViewModels
{
    public class ConfirmEmailViewModel : MailViewModel
    {
        public string ConfirmationCode { get; set; }
        public ApplicationUser User { get; set; }
        public string ConfirmEmailUrl { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace userManagement.Emails.ViewModels
{
    public class MailViewModel
    {
        public string AppUrl { get; set; }
        public string AccentColor { get; set; }

        public MailViewModel()
        {
            AccentColor = "#2196F3";
        }
    }
}

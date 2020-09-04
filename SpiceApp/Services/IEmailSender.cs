using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Services
{
    public interface IEmailSender
    {
        void SendMailkit(string name, string mailTo);
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}

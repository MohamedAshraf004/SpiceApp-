using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SpiceApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace SpiceApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> logger;

        public EmailSender(IOptions<EmailOptions> emailOptions,ILogger<EmailSender> logger)
        {
            Options = emailOptions.Value;
            this.logger = logger;
        }

        public EmailOptions Options { get; set; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public void SendMailkit(string name , string mailTo)
        {
            //Instantiaite message
            var message = new MimeMessage();
            //From Address
            message.From.Add(new MailboxAddress("SpicyRest", "spicy@spicy.com"));
            //To Address
            message.To.Add(new MailboxAddress(name , "lionelmido004@gmail.com"));
            //Subject
            message.Subject = "Your Order Has Been Confirmed";
            //Body
            message.Body = new TextPart("plain")
            {
                Text = "This is sent by mailkit."
            };
            //Configure And Send Email
            using var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("rajaraman6195@gmail.com", "Rajarman@123");
            client.Send(message);
            client.Disconnect(true);
            //client.ServerCertificateValidationCallback=

        }

        private Task Execute(string sendGridKey, string subject, string message, string email)
        {
            var client = new SendGridClient(sendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("Admin@Spicy.com", "Spicy Restaurant"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            try
            {
                return client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Task.CompletedTask;
            }
        }
    }
}

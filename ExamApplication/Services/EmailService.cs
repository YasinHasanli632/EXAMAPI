using ExamApplication.Interfaces.Services;
using ExamApplication.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailOptions)
        {
            _emailSettings = emailOptions.Value;
        }

        public async Task SendAsync(
     string toEmail,
     string subject,
     string htmlBody,
     CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new Exception("Göndəriləcək email ünvanı boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(_emailSettings.Host))
                throw new Exception("Email host təyin edilməyib.");

            if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
                throw new Exception("SenderEmail təyin edilməyib.");

            if (string.IsNullOrWhiteSpace(_emailSettings.Username))
                throw new Exception("Email username təyin edilməyib.");

            if (string.IsNullOrWhiteSpace(_emailSettings.Password))
                throw new Exception("Email password təyin edilməyib.");

            using var message = new MailMessage();
            message.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
            message.To.Add(toEmail.Trim());
            message.Subject = subject;
            message.Body = htmlBody;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = _emailSettings.UseSsl
            };

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await client.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                throw new Exception($"SMTP xətası: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Email göndərilmədi: {ex.Message}");
            }
        }
    }
}

using System.Net;
using System.Net.Mail;
using API.Interfaces;

namespace API.Services
{
public class EmailService : IEmailService
{
    private readonly string _fromEmail;
    private readonly string _emailPassword;

    public EmailService(string fromEmail, string emailPassword)
    {
        _fromEmail = fromEmail;
        _emailPassword = emailPassword;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using (var client = new SmtpClient("smtp.gmail.com", 587))
        {
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_fromEmail, _emailPassword);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}
}
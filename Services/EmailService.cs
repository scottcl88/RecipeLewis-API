using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace RecipeLewis.Services;

public interface IEmailService
{
    Task Send(string to, string subject, string html);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task Send(string to, string subject, string html)
    {
        string FROM = "support@recipelewis.com";
        string SMTP_USERNAME = _configuration["AWSSmtpUsername"] ?? "";
        string SMTP_PASSWORD = _configuration["AWSSmtpPassword"] ?? "";
        string HOST = "email-smtp.us-east-2.amazonaws.com";

        int PORT = 587;

        // Create and build a new MailMessage object
        MailMessage mailMessage = new MailMessage();
        mailMessage.IsBodyHtml = true;
        mailMessage.From = new MailAddress(FROM, "Recipe Lewis Support");
        mailMessage.To.Add(new MailAddress(to));
        mailMessage.Subject = subject;
        mailMessage.Body = html;

        using (var client = new SmtpClient(HOST, PORT))
        {
            client.Credentials =
                new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
            client.EnableSsl = true;
            await client.SendMailAsync(mailMessage);
        }
    }

    public async Task SendToBusinessSmtp(IConfiguration configuration, string fromEmail, string name, string subject, string message)
    {
        string TO = "support@surf-n-eat.com";
        string SMTP_USERNAME = configuration["AWSSmtpUsername"] ?? "";
        string SMTP_PASSWORD = configuration["AWSSmtpPassword"] ?? "";
        string HOST = "email-smtp.us-east-2.amazonaws.com";

        int PORT = 587;
        string htmlBody = @"<html>
                                    <head></head>
                                    <body>
                                      <p>" + message + @"</p>
                                    </body>
                                 </html>";

        // Create and build a new MailMessage object
        MailMessage mailMessage = new MailMessage();
        mailMessage.IsBodyHtml = true;
        mailMessage.From = new MailAddress(fromEmail, name);
        mailMessage.To.Add(new MailAddress(TO));
        mailMessage.Subject = subject;
        mailMessage.Body = htmlBody;

        using (var client = new SmtpClient(HOST, PORT))
        {
            client.Credentials =
                new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
            client.EnableSsl = true;
            await client.SendMailAsync(mailMessage);
        }
    }
}
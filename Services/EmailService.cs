using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RecipeLewis.Models;
using System.Net;
using System.Net.Mail;

namespace RecipeLewis.Services;

public interface IEmailService
{
    Task Send(string to, string subject, string html);
}

public class EmailService : IEmailService
{
    private readonly AppSettings _appSettings;
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _configuration = configuration;
    }

    public async Task Send(string to, string subject, string html)
    {
        string FROM = "support@recipelewis.com";
        string SMTP_USERNAME = _configuration["AWSSmtpUsername"];
        string SMTP_PASSWORD = _configuration["AWSSmtpPassword"];
        string HOST = "email-smtp.us-east-2.amazonaws.com";

        int PORT = 587;

        // Create and build a new MailMessage object
        MailMessage mailMessage = new MailMessage();
        mailMessage.IsBodyHtml = true;
        mailMessage.From = new MailAddress(FROM, "Recipe Lewis Support");
        mailMessage.To.Add(new MailAddress(to));
        mailMessage.Subject = subject;
        mailMessage.Body = html;

        try
        {
            using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
            {
                client.Credentials =
                    new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
                client.EnableSsl = true;
                await client.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        // create message
        //var email = new MimeMessage();
        //email.From.Add(MailboxAddress.Parse(from ?? _appSettings.EmailFrom));
        //email.To.Add(MailboxAddress.Parse(to));
        //email.Subject = subject;
        //email.Body = new TextPart(TextFormat.Html) { Text = html };

        //// send email
        //using var smtp = new SmtpClient();
        //smtp.Connect(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.StartTls);
        //smtp.Authenticate(_appSettings.SmtpUser, _appSettings.SmtpPass);
        //smtp.Send(email);
        //smtp.Disconnect(true);
    }

    public async Task SendToBusinessSmtp(IConfiguration configuration, string fromEmail, string name, string subject, string message)
    {
        String TO = "support@surf-n-eat.com";
        String SMTP_USERNAME = configuration["AWSSmtpUsername"];
        String SMTP_PASSWORD = configuration["AWSSmtpPassword"];
        String HOST = "email-smtp.us-east-2.amazonaws.com";

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

        using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
        {
            client.Credentials =
                new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
            client.EnableSsl = true;
            await client.SendMailAsync(mailMessage);
        }
    }
}
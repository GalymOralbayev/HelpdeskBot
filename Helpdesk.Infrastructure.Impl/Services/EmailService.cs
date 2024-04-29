using Helpdesk.Application.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Helpdesk.Infrastructure.Impl.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderName;
    private readonly string _senderEmail;
    private readonly string _password;

    public EmailService(IConfiguration configuration)
    {
        var emailConfig = configuration.GetSection("EmailSettings");
        _smtpServer = emailConfig["MailServer"];
        _smtpPort = int.Parse(emailConfig["MailPort"]);
        _senderName = emailConfig["SenderName"];
        _senderEmail = emailConfig["Sender"];
        _password = emailConfig["Password"];
    }

    public void SendEmail(string recipientEmail, string subject, string message) {
        using var client = new SmtpClient(_smtpServer, _smtpPort);
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(_senderEmail, _password);
        client.EnableSsl = true;

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_senderEmail, _senderName),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };
        mailMessage.To.Add(recipientEmail);

        client.Send(mailMessage);
    }
}

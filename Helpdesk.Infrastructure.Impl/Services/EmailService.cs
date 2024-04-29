using Helpdesk.Application.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using Helpdesk.Infrastructure.Impl.Options;
using Microsoft.Extensions.Options;

namespace Helpdesk.Infrastructure.Impl.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderName;
    private readonly string _senderEmail;
    private readonly string _password;

    public EmailService(IOptions<EmailOptions> options)
    {
        _smtpServer = options.Value.MailServer;
        _smtpPort = options.Value.MailPort;
        _senderName = options.Value.SenderName;
        _senderEmail = options.Value.Sender;
        _password = options.Value.Password;
    }

    public void SendEmail(string recipientEmail, string subject, string message) {
        var fromAddress = new MailAddress(_senderEmail, _senderName);
        var toAddress = new MailAddress(recipientEmail);
        using var client = new SmtpClient(_smtpServer, _smtpPort);
        client.EnableSsl = true;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(_senderEmail, _password);

        using var mailMessage = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = message
        };
        client.Send(mailMessage);
    }
}

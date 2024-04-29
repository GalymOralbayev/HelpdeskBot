namespace Helpdesk.Application.Services;

public interface IEmailService { 
    void SendEmail(string recipientEmail, string subject, string message);
}
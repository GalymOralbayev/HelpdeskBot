namespace Helpdesk.Infrastructure.Impl.Options;

public class EmailOptions {
    public static string SectionName { get; } = "EmailSettings";
    public string MailServer { get; set; }
    public int MailPort { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
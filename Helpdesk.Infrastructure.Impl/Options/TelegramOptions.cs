using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Infrastructure.Impl.Options;

public class TelegramOptions {
    /// <summary> Configuration section name </summary>
    public static string SectionName { get; } = "Telegram";
    
    /// <summary> Token of the bot </summary>
    [Required]
    public string BotToken { get; set; }
    
    /// <summary> Whether to use webhook (otherwise long-pooling is used) </summary>
    public bool UseWebhook { get; set; } = false;
    
    /// <summary> Webhook URL (this app) </summary>
    public string WebhookUrl { get; set; } = string.Empty;
}
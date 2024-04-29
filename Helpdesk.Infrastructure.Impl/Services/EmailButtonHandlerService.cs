using Helpdesk.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Helpdesk.Infrastructure.Impl.Services;

public class EmailButtonStates {
    public const string AwaitingEmailAddress = "await_email";
    public const string AwaitingMessage = "await_message";
}

public class EmailButtonHandlerService {
    private readonly Dictionary<long, string?> _emailChatStates = new Dictionary<long, string?>();

    private readonly IServiceScopeFactory _scopeFactory;

    public EmailButtonHandlerService(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }


    public async Task<bool> HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        _emailChatStates.TryAdd(message.Chat.Id, null);

        switch (_emailChatStates[message.Chat.Id]) {
            case EmailButtonStates.AwaitingEmailAddress:
                await ProcessEmail(botClient, message, ct);
                return false;
            case EmailButtonStates.AwaitingMessage:
                await SendEmail(botClient, message, ct);
                return true;
            default: await Main(botClient, message, ct);
                return false;
        }
    }

    private async Task Main(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        using var scope = _scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var user = await userService.TryGetOrInsertUser(message.From?.Username, ct);

        if (user != null && !string.IsNullOrEmpty(user.Email)) {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Ваш e-mail адрес: {user.Email}. \nПожалуйста, введите текст обращения:",
                cancellationToken: ct
            );
            _emailChatStates[message.Chat.Id] = EmailButtonStates.AwaitingMessage;
            return;
        }

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Пожалуйста, введите ваш email:",
            cancellationToken: ct
        );
        _emailChatStates[message.Chat.Id] = EmailButtonStates.AwaitingEmailAddress;
    }

    private async Task SendEmail(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        using var scope = _scopeFactory.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var user = await userService.TryGetOrInsertUser(message.From.Username, ct);
        
        emailService.SendEmail(user.Email, "Test bot", message.Text!);
        _emailChatStates[message.Chat.Id] = null;
    }

    private async Task ProcessEmail(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        if (IsValidEmail(message.Text!)) {
            using var scope = _scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            await userService.UpdateUserEmail(message.From?.Username, message.Text, ct);
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Спасибо, ваш email сохранён. Введите текст обращения: ",
                cancellationToken: ct
            );
            _emailChatStates[message.Chat.Id] = EmailButtonStates.AwaitingMessage;
        }
        else {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Некорректный формат email, попробуйте ещё раз.",
                cancellationToken: ct
            );
            _emailChatStates[message.Chat.Id] = EmailButtonStates.AwaitingEmailAddress; // Повторное ожидание ввода
        }
    }

    private bool IsValidEmail(string email) {
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch {
            return false;
        }
    }
}
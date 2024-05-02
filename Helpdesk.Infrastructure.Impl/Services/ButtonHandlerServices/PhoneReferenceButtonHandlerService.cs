using System.Text.RegularExpressions;
using Helpdesk.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Helpdesk.Infrastructure.Impl.Services.ButtonHandlerServices;

public class PhoneReferenceButtonStates {
    public const string AwaitingSearchText = "await_text";
}

public class PhoneReferenceButtonHandlerService {
    private readonly Dictionary<long, string?> _phoneChatStates = new Dictionary<long, string?>();

    private readonly IServiceScopeFactory _scopeFactory;

    public PhoneReferenceButtonHandlerService(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }


    public async Task<bool> HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        _phoneChatStates.TryAdd(message.Chat.Id, null);

        switch (_phoneChatStates[message.Chat.Id]) {
            case PhoneReferenceButtonStates.AwaitingSearchText:
                await SearchText(botClient, message, ct);
                return true;
            default:
                await Main(botClient, message, ct);
                return false;
        }
    }

    private async Task Main(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text:
            $"Напишите ключевое слово для поиска сотрудника( это может быть название департамента, должность или ФИО сотрудника):",
            cancellationToken: ct
        );
        _phoneChatStates[message.Chat.Id] = PhoneReferenceButtonStates.AwaitingSearchText;
    }

    private async Task SearchText(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        using var scope = _scopeFactory.CreateScope();
        var phoneReferenceRepository = scope.ServiceProvider.GetRequiredService<IPhoneReferenceRepository>();
        var phones = await phoneReferenceRepository.Search(CleanString(message.Text!), ct);

        if (phones.Any()) {
            const int batchSize = 5;
            var batches = phones.Select((phone, index) => new { phone, index })
                .GroupBy(x => x.index / batchSize)
                .Select(group => group.Select(x => x.phone).ToList())
                .ToList();

            foreach (var batch in batches) {
                var resText = batch.Aggregate(string.Empty,
                    (current, phoneReference) => current + phoneReference.ToString() + "\n\n");

                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: resText,
                    cancellationToken: ct
                );
            }
        }
        else {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Ничего не найдено",
                cancellationToken: ct
            );
        }

        _phoneChatStates[message.Chat.Id] = null;
    }
    
    private string CleanString(string input) {
        if (string.IsNullOrEmpty(input))
            return input;
        
        var cleaned = Regex.Replace(input, @"\s+", " ");
    
        return cleaned.Trim();
    }
}
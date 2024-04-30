using Helpdesk.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Helpdesk.Infrastructure.Impl.Services.ButtonHandlerServices;

public class RoomButtonStates {
    public const string AwaitingArticleNumber = "await_article_num";
    public const string AwaitingRoomNumber = "await_room_num";
}

public class RoomButtonHandlerService {
    private readonly Dictionary<long, string?> _roomChatStates = new Dictionary<long, string?>();
    private readonly Dictionary<long, string?> _articleNumbers = new Dictionary<long, string?>();
    private readonly IServiceScopeFactory _scopeFactory;

    public RoomButtonHandlerService(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }


    public async Task<bool> HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        _roomChatStates.TryAdd(message.Chat.Id, null);

        switch (_roomChatStates[message.Chat.Id]) {
            case RoomButtonStates.AwaitingArticleNumber:
                await RequestRoomNumber(botClient, message, ct);
                return false;
            case RoomButtonStates.AwaitingRoomNumber:
                await SearchRoom(botClient, message, ct);
                return true;
            default:
                await RequestArticleNumber(botClient, message, ct);
                return false;
        }
    }

    private async Task RequestArticleNumber(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        _articleNumbers.TryAdd(message.Chat.Id, null);

        using var scope = _scopeFactory.CreateScope();
        var articleRepository = scope.ServiceProvider.GetRequiredService<IArticleRepository>();
        var articles = await articleRepository.GetAll(ct);
        
        
        var resText = articles.Aggregate(string.Empty,
            (current, article) => current + article.ToString() + "\n\n");
        
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: resText,
            cancellationToken: ct
        );
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Введите номер блока",
            cancellationToken: ct
        );
        _roomChatStates[message.Chat.Id] = RoomButtonStates.AwaitingArticleNumber;
    }

    private async Task RequestRoomNumber(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        _articleNumbers[message.Chat.Id] = message.Text;
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Введите номер комнаты: ",
            cancellationToken: ct
        );
        _roomChatStates[message.Chat.Id] = RoomButtonStates.AwaitingRoomNumber;
    }

    private async Task SearchRoom(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        using var scope = _scopeFactory.CreateScope();
        var roomRepository = scope.ServiceProvider.GetRequiredService<IRoomRepository>();
        var rooms = await roomRepository.Search(x => x.Article.Number == _articleNumbers[message.Chat.Id] 
                                                      && x.RoomNumber == message.Text, ct);

        if (rooms.Any()) {
            const int batchSize = 5;
            var batches = rooms.Select((phone, index) => new { phone, index })
                .GroupBy(x => x.index / batchSize)
                .Select(group => group.Select(x => x.phone).ToList())
                .ToList();

            foreach (var batch in batches) {
                var resText = batch.Aggregate(string.Empty,
                    (current, room) => current + room.ToString() + "\n\n");

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

        _roomChatStates[message.Chat.Id] = null;
        _articleNumbers[message.Chat.Id] = null;
    }
}
using CSharpFunctionalExtensions;
using Helpdesk.Infrastructure.Impl.Options;
using Helpdesk.Application.Services;
using Helpdesk.Domain.Constants;
using Helpdesk.Infrastructure.Impl.Services.ButtonHandlerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Helpdesk.Infrastructure.Impl.Services;

/// <summary> Реализация сервиса взаимодействия с Telegram </summary>
public class TelegramService : ITelegramService {
    private readonly ILogger<TelegramService> _logger;
    private readonly TelegramBotClient _botClient;
    private readonly TelegramOptions _options;
    private readonly CancellationTokenSource _cts = new();
    private readonly EmailButtonHandlerService _emailButtonHandlerService;
    private readonly PhoneReferenceButtonHandlerService _phoneReferenceButtonHandlerService;
    private readonly RoomButtonHandlerService _roomButtonHandlerService;
    private readonly InstructionButtonHandlerService _instructionButtonHandlerService;
    private readonly Dictionary<long, string?> _chatStates = new Dictionary<long, string?>();

    public TelegramService(
        ILogger<TelegramService> logger, 
        IOptions<TelegramOptions> options,
        EmailButtonHandlerService emailButtonHandlerService,
        PhoneReferenceButtonHandlerService phoneReferenceButtonHandlerService, RoomButtonHandlerService roomButtonHandlerService, InstructionButtonHandlerService instructionButtonHandlerService) {
        
        _logger = logger;
        this._emailButtonHandlerService = emailButtonHandlerService;
        this._phoneReferenceButtonHandlerService = phoneReferenceButtonHandlerService;
        _roomButtonHandlerService = roomButtonHandlerService;
        _instructionButtonHandlerService = instructionButtonHandlerService;
        this._options = options.Value;
        this._botClient = new TelegramBotClient(_options.BotToken);
    }

    /// <summary> Отправить исходящее сообщение в Telegram </summary> 
    public async Task<Result<Message>> SendMessageAsync(long chatId, string message = "Выберите опцию", int? replyToMessageId = null) {
        if(message.Length > 4096) {
            return Result.Failure<Message>("Сообщение превышает максимально допустимую длину в 4096 символов");
        }

        var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[] {
            new KeyboardButton[] { Buttons.Help},
            new KeyboardButton[] { Buttons.SendEMail },
            new KeyboardButton[] { Buttons.PhoneReferences },
            new KeyboardButton[] { Buttons.GetRooms },
            new KeyboardButton[] { Buttons.GetInstructions },
            new KeyboardButton[] { Buttons.HelperContacts },
            new KeyboardButton[] { Buttons.UniversityMap },
        }) {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };

        var sentMessage = await _botClient.SendTextMessageAsync(
            chatId: chatId, 
            text: message,
            replyMarkup: replyKeyboardMarkup,
            cancellationToken: _cts.Token);

        return Result.Success(sentMessage);
    }
    
    /// <summary> Начать получение сообщений </summary>
    public Task StartReceivingAsync() {
        var cts = new CancellationTokenSource();
        var receiverOptions = new ReceiverOptions {
            AllowedUpdates = { } // Получать все типы обновлений
        };

        _botClient.StartReceiving(
            HandleIncomingMessageAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token);
        
        return Task.CompletedTask;
    }

    public async Task StopReceivingAsync(CancellationToken cancellationToken) {
        // Remove webhook on app shutdown
        _logger.LogInformation("Removing webhook");
        await _botClient.DeleteWebhookAsync(dropPendingUpdates: true, cancellationToken: cancellationToken);
    }
    
    /// <summary> Обработать входящее сообщение </summary>
    public async Task HandleIncomingMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        // Обрабатываем только сообщения
        if (update.Type != UpdateType.Message || update.Message == null) {
            this._logger.LogWarning("Skipping update of type {updateType}", update.Type);
            return;
        }

        var message = update.Message;
        using var scope = _logger.BeginScope($"message-{message.MessageId}");
        _logger.LogInformation("Incoming message {messageId} received from user {user} (chat {chat})",
            message.MessageId,
            message.From is not null
                ? $"@{message.From.Username} ({message.From.FirstName + message.From.LastName})"
                : "unknown",
            message.Chat.Id);
        
        try {
            // Специфичные обработчики разных типов сообщений
            Result messageHandlingResult = message.Type switch {
                MessageType.Text => await HandleTextMessageAsync(botClient, message, cancellationToken),
                _ => await HandleNotSupportedMessageAsync(message)
            };

            // Обработка ошибок
            if (messageHandlingResult.IsFailure) {
                _logger.LogError("Failed to handle message {messageId}. Details: {error}",
                    message.MessageId, messageHandlingResult.Error);
            }
            
            // Логируем успешную обработку сообщения
            _logger.LogInformation("Message {messageId} successfully handled", message.MessageId);
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary> Обработчик текстовых сообщений </summary>
    private async Task<Result> HandleTextMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        _chatStates.TryAdd(message.Chat.Id, null);
        var switchConstText = _chatStates[message.Chat.Id] ?? message.Text;
        switch (switchConstText) {
            case Buttons.Help: 
                await SendMessageAsync(message.Chat.Id, ConstantStings.HelpText); break;
            case Buttons.SendEMail: 
                _chatStates[message.Chat.Id] = Buttons.SendEMail;
                if (!await _emailButtonHandlerService.HandleMessageAsync(botClient, message, ct))
                    return Result.Success();
                _chatStates[message.Chat.Id] = null;
                await SendMessageAsync(message.Chat.Id, "Ваше письмо отправлено! \nВыберите новую опцию:");
                return Result.Success();
            case Buttons.PhoneReferences: 
                _chatStates[message.Chat.Id] = Buttons.PhoneReferences;
                if (!await _phoneReferenceButtonHandlerService.HandleMessageAsync(botClient, message, ct))
                    return Result.Success();
                _chatStates[message.Chat.Id] = null;
                await SendMessageAsync(message.Chat.Id);
                return Result.Success();
            case Buttons.GetRooms: 
                _chatStates[message.Chat.Id] = Buttons.GetRooms;
                if (!await _roomButtonHandlerService.HandleMessageAsync(botClient, message, ct))
                    return Result.Success();
                _chatStates[message.Chat.Id] = null;
                await SendMessageAsync(message.Chat.Id);
                return Result.Success();
            case Buttons.GetInstructions: 
                _chatStates[message.Chat.Id] = Buttons.GetInstructions;
                if (!await _instructionButtonHandlerService.HandleMessageAsync(botClient, message, ct))
                    return Result.Success();
                _chatStates[message.Chat.Id] = null;
                await SendMessageAsync(message.Chat.Id);
                return Result.Success();
            case Buttons.HelperContacts: await SendMessageAsync(message.Chat.Id, ConstantStings.TechnicalSupportContacts); break;
            case Buttons.UniversityMap: await SendMessageAsync(message.Chat.Id, ConstantStings.MapLink); break;
            case Buttons.Admin: await AdminButtonHandler(ct); break;
        }
        
        await SendMessageAsync(message.Chat.Id);
        _logger.LogDebug("Text message {messageId} received.", message.MessageId);

        return Result.Success();
    }

    /// <summary> Обработчик сообщений, которые не поддерживаются </summary>
    private async Task<Result> HandleNotSupportedMessageAsync(Message message) {
        await SendMessageAsync(message.Chat.Id, $"<b>Внимание</b>: Данный вид сообщений не поддерживается!");
        return Result.Failure($"Message type {message.Type} is not supported");
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken) {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }

    private Task AdminButtonHandler(CancellationToken ct) {
        return Task.CompletedTask;
    }
}
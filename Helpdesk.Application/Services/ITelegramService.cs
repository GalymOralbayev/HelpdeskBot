using CSharpFunctionalExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace Helpdesk.Application.Services;

public interface ITelegramService { 
    public Task StartReceivingAsync();
    /// Отправить сообщение в чат
    Task<Result<Message>> SendMessageAsync(long chatId, string message, int? replyToMessageId);
    
    /// Обработать входящее сообщение
    Task HandleIncomingMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
}
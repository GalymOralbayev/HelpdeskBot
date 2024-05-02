using Helpdesk.Domain.Constants;
using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Helpdesk.Infrastructure.Impl.Services.ButtonHandlerServices;

public class InstructionButtonStates { //TODO переделать в енам, либо класс констнат состоянии чата (chatState, buttonState)
    public const string AwaitingInstructionName = "await_instruction_name";
}

public class InstructionButtonHandlerService {
    private readonly Dictionary<long, string?> _instructionChatStates = new Dictionary<long, string?>();
    private readonly IServiceScopeFactory _scopeFactory;

    public InstructionButtonHandlerService(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    public async Task<bool> HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        _instructionChatStates.TryAdd(message.Chat.Id, null);

        switch (_instructionChatStates[message.Chat.Id]) {
            case InstructionButtonStates.AwaitingInstructionName:
                await ShowInstruction(botClient, message, ct);
                return true;
            default:
                await RequestInstructionName(botClient, message, ct);
                return false;
        }
    }

    private async Task RequestInstructionName(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        using var scope = _scopeFactory.CreateScope();
        var instructionRepository = scope.ServiceProvider.GetRequiredService<IInstructionRepository>();
        var instructions = await instructionRepository.GetAll(ct);
        
        var keyboardButtons = instructions.Select(instruction => 
            new[] { new KeyboardButton(instruction?.Name ?? "Пусто...") }
        ).ToArray();

        var replyKeyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons) {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };
        
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Выберите инструкцию",
            replyMarkup: replyKeyboardMarkup,
            cancellationToken: ct
        );
        _instructionChatStates[message.Chat.Id] = InstructionButtonStates.AwaitingInstructionName;
    }

    private async Task ShowInstruction(ITelegramBotClient botClient, Message message, CancellationToken ct) {
        using var scope = _scopeFactory.CreateScope();
        var instructionRepository = scope.ServiceProvider.GetRequiredService<IInstructionRepository>();
        var instruction = await instructionRepository.GetByName(message.Text, ct);

        if (instruction != null && instruction.Steps.Any()) {
            foreach (var step in instruction.Steps.OrderBy(x => x.StepNumber)) {
                
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"Шаг {step.StepNumber}: {step.StepText}",
                    cancellationToken: ct
                );
                if (step.Photos.Any()) {
                    await SendImages(step.Photos, botClient, message, ct);
                }
            }
        }
        else {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Ничего не найдено",
                cancellationToken: ct
            );
        }

        _instructionChatStates[message.Chat.Id] = null;
    }

    private async Task SendImages(List<Photo> photos, ITelegramBotClient botClient, Message message, CancellationToken ct) {
        foreach (var photo in photos) {
            using var stream = new MemoryStream(photo.Content);
            var inputFile = InputFile.FromStream(stream, "image.jpg");
            await botClient.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo:   inputFile,
                cancellationToken: ct
            );
        }        
    }
}
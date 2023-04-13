using StepFlow.Contracts;

namespace QuizMaker.TelegramBot.Steps;

public class TextMessage : IStep
{
    public TextMessage(TelegramBotService botService)
    {
        _botService = botService;
    }

    public string UserId { get; set; } = default!;

    public string Message { get; set; } = default!;

    public async Task ExecuteAsync()
    {
        await _botService.SendMessage(UserId, Message);
    }

    private readonly TelegramBotService _botService;
}
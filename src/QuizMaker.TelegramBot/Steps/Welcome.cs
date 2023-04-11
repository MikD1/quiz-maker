using StepFlow.Contracts;

namespace QuizMaker.TelegramBot.Steps;

public class Welcome : IStep
{
    public Welcome(TelegramBotService botService)
    {
        _botService = botService;
    }

    public string UserId { get; set; } = default!;

    public async Task ExecuteAsync()
    {
        string message = "Здравствуй, Игоренок! Твой квест начинается. . .";
        await _botService.SendMessage(UserId, message);
    }

    private readonly TelegramBotService _botService;
}
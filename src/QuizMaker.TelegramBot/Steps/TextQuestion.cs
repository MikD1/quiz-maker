using StepFlow.Contracts;

namespace QuizMaker.TelegramBot.Steps;

public class TextQuestion : IStep
{
    public TextQuestion(TelegramBotService botService)
    {
        _botService = botService;
    }

    public string UserId { get; set; } = default!;

    public async Task ExecuteAsync()
    {
        string question = $"Это новый текстовый вопрос: {Guid.NewGuid()}";
        await _botService.SendMessage(UserId, question);
    }

    private readonly TelegramBotService _botService;
}
using StepFlow.Contracts;

namespace QuizMaker.TelegramBot.Steps;

public class TextQuestion : IStep
{
    public TextQuestion(TelegramBotService botService, IWorkflowHost workflowHost)
    {
        _botService = botService;
        _workflowHost = workflowHost;
    }

    public string UserId { get; set; } = default!;

    public string Question { get; set; } = default!;

    public string CorrectAnswer { get; set; } = default!;

    public async Task ExecuteAsync()
    {
        _workflowHost.EventPublished += OnEventPublished;

        await _botService.SendMessage(UserId, Question);

        bool hasCorrectAnswer = false;
        while (!hasCorrectAnswer)
        {
            string answer = await WaitAnswer();
            if (string.Equals(CorrectAnswer, answer, StringComparison.CurrentCultureIgnoreCase))
            {
                await _botService.SendMessage(UserId, "Верно!");
                hasCorrectAnswer = true;
            }
            else
            {
                await _botService.SendMessage(UserId, "Что-то пошло не так... попробуй еще раз!");
                ResetAnswer();
            }
        }

        _workflowHost.EventPublished -= OnEventPublished;
    }

    private void OnEventPublished(object? sender, WorkflowEvent @event)
    {
        if (@event.EventName is "UserMessage" && @event.EventKey == UserId)
        {
            lock (_sync)
            {
                _answer = @event.EventData;
            }
        }
    }

    private async Task<string> WaitAnswer()
    {
        string? answer = null;
        while (answer is null)
        {
            lock (_sync)
            {
                answer = _answer;
            }

            await Task.Delay(100);
        }

        return answer;
    }

    private void ResetAnswer()
    {
        lock (_sync)
        {
            _answer = null;
        }
    }

    private readonly object _sync = new();
    private readonly TelegramBotService _botService;
    private readonly IWorkflowHost _workflowHost;
    private string? _answer;
}
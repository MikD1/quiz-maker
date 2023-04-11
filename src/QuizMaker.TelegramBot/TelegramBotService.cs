using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StepFlow.Contracts;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QuizMaker.TelegramBot;

public class TelegramBotService : IHostedService
{
    public TelegramBotService(ILogger<TelegramBotService> logger, IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        string token = _configuration["QuizMakerTelegramBotToken"]!;
        _botClient = new TelegramBotClient(token);
        _receivingCts = new CancellationTokenSource();

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _botClient.StartReceiving(HandleUpdate, HandleError, receiverOptions, _receivingCts.Token);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _receivingCts.Cancel();
        return Task.CompletedTask;
    }

    public async Task<string> GetBotInfo()
    {
        User user = await _botClient.GetMeAsync();
        return $"bot: {user.Id}, {user.FirstName}";
    }

    public async Task SendMessage(string userId, string message)
    {
        await _botClient.SendTextMessageAsync(userId, message);
        _logger.LogInformation($"Message '{message}' send to user '{userId}'");
    }

    private async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message?.Text is null)
        {
            return;
        }

        string userId = update.Message.Chat.Id.ToString();
        string messageText = update.Message.Text;

        _logger.LogInformation($"HandleUpdate: '{userId}' : '{messageText}'");

        switch (messageText)
        {
            case "/start":
                await StartQuizWorkflowForUser(userId);
                break;
            default:
                await PublishWorkflowEvent(userId, messageText);
                break;
        }
    }

    private Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        string errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogError(errorMessage);
        return Task.CompletedTask;
    }

    private async Task StartQuizWorkflowForUser(string userId)
    {
        UserQuizData data = new()
        {
            UserId = userId
        };

        using IServiceScope scope = _scopeFactory.CreateScope();
        IUserQuizDataStore dataStore = scope.ServiceProvider.GetRequiredService<IUserQuizDataStore>();
        IWorkflowHost workflowHost = scope.ServiceProvider.GetRequiredService<IWorkflowHost>();

        await dataStore.Save(data);
        string workflowId = workflowHost.RunWorkflow("QuizWorkflow", data);
        _logger.LogInformation($"Quiz workflow '{workflowId}' started for user '{userId}'");
    }

    private async Task PublishWorkflowEvent(string userId, string userText)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IWorkflowHost workflowHost = scope.ServiceProvider.GetRequiredService<IWorkflowHost>();
        IUserQuizDataStore dataStore = scope.ServiceProvider.GetRequiredService<IUserQuizDataStore>();

        UserQuizData? userData = await dataStore.Get(userId);
        if (userData is null)
        {
            return;
        }

        workflowHost.PublishEvent($"{userData.UserId}:UserMessage", userText);
    }

    private readonly ILogger<TelegramBotService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private ITelegramBotClient _botClient = default!;
    private CancellationTokenSource _receivingCts = default!;
}
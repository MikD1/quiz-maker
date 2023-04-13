using QuizMaker.TelegramBot;
using QuizMaker.TelegramBot.Steps;
using StepFlow.Contracts;
using StepFlow.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IUserQuizDataStore, UserQuizDataStoreInMemory>();
builder.Services.AddSingleton<TelegramBotService>();
builder.Services.AddHostedService<TelegramBotService>(provider => provider.GetService<TelegramBotService>()!);

builder.Services.AddStepFlow();
builder.Services.AddTransient<TextMessage>();
builder.Services.AddTransient<TextQuestion>();

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

IWorkflowHost workflowHost = app.Services.GetService<IWorkflowHost>()!;
workflowHost.RegisterWorkflow<QuizWorkflow, UserQuizData>();

app.MapControllers();
app.Run();
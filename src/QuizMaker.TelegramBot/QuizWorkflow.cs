using QuizMaker.TelegramBot.Steps;
using StepFlow.Contracts;

namespace QuizMaker.TelegramBot;

public class QuizWorkflow : IWorkflow<UserQuizData>
{
    public string Name => "QuizWorkflow";

    public void Build(IWorkflowBuilder<UserQuizData> builder)
    {
        builder
            .Step<Welcome>(x => x
                .Input(step => step.UserId, data => data.UserId))
            .Step<TextQuestion>(x => x
                .Input(step => step.UserId, data => data.UserId))
            .Step<TextQuestion>(x => x
                .Input(step => step.UserId, data => data.UserId))
            .Step<TextQuestion>(x => x
                .Input(step => step.UserId, data => data.UserId));
    }
}
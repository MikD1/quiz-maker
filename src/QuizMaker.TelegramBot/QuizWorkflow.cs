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
                .Input(step => step.UserId, data => data.UserId)
                .Input(step => step.Question, _ => "Вопрос 1")
                .Input(step => step.CorrectAnswer, _ => "123"))
            .Step<TextQuestion>(x => x
                .Input(step => step.UserId, data => data.UserId)
                .Input(step => step.Question, _ => "Вопрос 2")
                .Input(step => step.CorrectAnswer, _ => "abc"))
            .Step<TextQuestion>(x => x
                .Input(step => step.UserId, data => data.UserId)
                .Input(step => step.Question, _ => "Вопрос 2")
                .Input(step => step.CorrectAnswer, _ => "!@#"));
    }
}
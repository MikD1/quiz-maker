using QuizMaker.TelegramBot.Steps;
using StepFlow.Contracts;

namespace QuizMaker.TelegramBot;

public class QuizWorkflow : IWorkflow<UserQuizData>
{
    public string Name => "QuizWorkflow";

    public void Build(IWorkflowBuilder<UserQuizData> builder)
    {
        builder
            .Step<TextMessage>(x => x
                .Input(step => step.UserId, data => data.UserId)
                .Input(step => step.Message, _ => "Hello"))
            .Step<TextQuestion>(x => x
                .Input(step => step.UserId, data => data.UserId)
                .Input(step => step.Question, _ => "Вопрос 1")
                .Input(step => step.CorrectAnswer, _ => "123"))
            .WaitForEvent("UserPhoto", data => data.UserId)
            .Step<TextQuestion>(x => x
                .Input(step => step.UserId, data => data.UserId)
                .Input(step => step.Question, _ => "Вопрос 2")
                .Input(step => step.CorrectAnswer, _ => "abc"))
            .Step<TextQuestion>(x => x
                .Input(step => step.UserId, data => data.UserId)
                .Input(step => step.Question, _ => "Вопрос 3")
                .Input(step => step.CorrectAnswer, _ => "!@#"))
            .WaitForEvent("UserPhoto", data => data.UserId)
            .WaitForEvent("UserPhoto", data => data.UserId)
            .Step<TextMessage>(x => x
                .Input(step => step.UserId, data => data.UserId)
                .Input(step => step.Message, _ => "Good bay!"));
    }
}
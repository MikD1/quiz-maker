namespace QuizMaker.TelegramBot;

public interface IUserQuizDataStore
{
    public Task Save(UserQuizData userData);

    public Task<UserQuizData?> Get(string userId);
}
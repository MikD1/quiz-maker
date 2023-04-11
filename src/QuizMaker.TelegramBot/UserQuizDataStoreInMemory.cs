using System.Collections.Concurrent;

namespace QuizMaker.TelegramBot;

public class UserQuizDataStoreInMemory : IUserQuizDataStore
{
    public Task Save(UserQuizData userData)
    {
        _data[userData.UserId] = userData;
        return Task.CompletedTask;
    }

    public Task<UserQuizData?> Get(string userId)
    {
        _data.TryGetValue(userId, out UserQuizData? userData);
        return Task.FromResult(userData);
    }

    private readonly ConcurrentDictionary<string, UserQuizData> _data = new();
}
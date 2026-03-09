namespace reg_bot;

public class UserStore
{
    private readonly Dictionary<long, User> _users = [];
    
    public void SetUser(long userId, User user)
    {
        if(!_users.TryAdd(userId, user))
            _users[userId] = user;
    }

    public User? GetUser(long userId)
    {
        return _users.GetValueOrDefault(userId);
    }
}
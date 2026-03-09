namespace reg_bot;

public class User
{
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Mood MoodToday { get; set; }
}

public enum Mood
{
    None,
    Awesome,
    Good,
    Ok,
    Bad
}
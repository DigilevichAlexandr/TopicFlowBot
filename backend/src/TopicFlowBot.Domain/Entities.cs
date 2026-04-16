namespace TopicFlowBot.Domain;

public class User
{
    public int Id { get; set; }
    public long TelegramId { get; set; }
    public string Username { get; set; } = string.Empty;
    public List<Favorite> Favorites { get; set; } = [];
}

public class Topic
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Favorite> Favorites { get; set; } = [];
}

public class Favorite
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = default!;
    public int TopicId { get; set; }
    public Topic Topic { get; set; } = default!;
}

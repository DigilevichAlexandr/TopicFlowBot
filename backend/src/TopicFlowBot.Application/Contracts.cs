using TopicFlowBot.Domain;

namespace TopicFlowBot.Application;

public record TelegramAuthRequest(string InitData);
public record TelegramUserDto(long Id, string? Username, string? FirstName);
public record TelegramAuthResponse(TelegramUserDto User);
public record TopicDto(int Id, string Title, string Description);
public record CreateFavoriteRequest(int UserId, int TopicId);
public record FavoriteDto(int Id, int UserId, int TopicId, TopicDto Topic);

public interface ITelegramAuthService
{
    Task<TelegramAuthResponse> AuthenticateAsync(string initData, CancellationToken ct);
}

public interface ITopicService
{
    Task<IReadOnlyList<TopicDto>> SearchAsync(string query, CancellationToken ct);
}

public interface IFavoriteService
{
    Task AddAsync(CreateFavoriteRequest request, CancellationToken ct);
    Task<IReadOnlyList<FavoriteDto>> GetByUserAsync(int userId, CancellationToken ct);
}

public interface IUserRepository
{
    Task<User?> GetByTelegramIdAsync(long telegramId, CancellationToken ct);
    Task<User> AddAsync(User user, CancellationToken ct);
}

public interface ITopicRepository
{
    Task<IReadOnlyList<Topic>> SearchAsync(string query, CancellationToken ct);
    Task<Topic?> GetByIdAsync(int id, CancellationToken ct);
}

public interface IFavoriteRepository
{
    Task AddAsync(Favorite favorite, CancellationToken ct);
    Task<bool> ExistsAsync(int userId, int topicId, CancellationToken ct);
    Task<IReadOnlyList<Favorite>> GetByUserIdAsync(int userId, CancellationToken ct);
}

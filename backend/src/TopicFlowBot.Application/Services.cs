using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TopicFlowBot.Domain;

namespace TopicFlowBot.Application;

public class TelegramAuthService(
    IUserRepository userRepository,
    string botToken) : ITelegramAuthService
{
    public async Task<TelegramAuthResponse> AuthenticateAsync(string initData, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(initData))
        {
            throw new ArgumentException("initData is empty.");
        }

        var parsed = ParseInitData(initData);
        if (!parsed.TryGetValue("hash", out var actualHash))
        {
            throw new UnauthorizedAccessException("Missing hash in initData.");
        }

        var dataCheckString = string.Join('\n', parsed.Where(k => k.Key != "hash").OrderBy(k => k.Key).Select(k => $"{k.Key}={k.Value}"));
        var secretKey = HMACSHA256.HashData(Encoding.UTF8.GetBytes("WebAppData"), Encoding.UTF8.GetBytes(botToken));
        var expected = Convert.ToHexString(HMACSHA256.HashData(secretKey, Encoding.UTF8.GetBytes(dataCheckString))).ToLowerInvariant();
        if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(actualHash)))
        {
            throw new UnauthorizedAccessException("Invalid Telegram hash.");
        }

        var userJson = parsed.GetValueOrDefault("user") ?? throw new UnauthorizedAccessException("Missing Telegram user.");
        var user = JsonSerializer.Deserialize<TelegramUserDto>(userJson) ?? throw new UnauthorizedAccessException("Invalid Telegram user payload.");

        var dbUser = await userRepository.GetByTelegramIdAsync(user.Id, ct);
        if (dbUser is null)
        {
            dbUser = await userRepository.AddAsync(new User
            {
                TelegramId = user.Id,
                Username = user.Username ?? user.FirstName ?? $"user_{user.Id}"
            }, ct);
        }

        return new TelegramAuthResponse(new TelegramUserDto(dbUser.Id, dbUser.Username, user.FirstName));
    }

    private static Dictionary<string, string> ParseInitData(string initData) =>
        initData.Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Split('=', 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => Uri.UnescapeDataString(parts[0]), parts => Uri.UnescapeDataString(parts[1]));
}

public class TopicService(ITopicRepository topicRepository) : ITopicService
{
    public async Task<IReadOnlyList<TopicDto>> SearchAsync(string query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var topics = await topicRepository.SearchAsync(query, ct);
        return topics.Select(t => new TopicDto(t.Id, t.Title, t.Description)).ToList();
    }
}

public class FavoriteService(
    IFavoriteRepository favoriteRepository,
    ITopicRepository topicRepository) : IFavoriteService
{
    public async Task AddAsync(CreateFavoriteRequest request, CancellationToken ct)
    {
        var topic = await topicRepository.GetByIdAsync(request.TopicId, ct) ?? throw new KeyNotFoundException("Topic not found.");
        var exists = await favoriteRepository.ExistsAsync(request.UserId, request.TopicId, ct);
        if (exists)
        {
            return;
        }

        await favoriteRepository.AddAsync(new Favorite { UserId = request.UserId, TopicId = topic.Id }, ct);
    }

    public async Task<IReadOnlyList<FavoriteDto>> GetByUserAsync(int userId, CancellationToken ct)
    {
        var favorites = await favoriteRepository.GetByUserIdAsync(userId, ct);
        return favorites.Select(f => new FavoriteDto(f.Id, f.UserId, f.TopicId, new TopicDto(f.Topic.Id, f.Topic.Title, f.Topic.Description))).ToList();
    }
}

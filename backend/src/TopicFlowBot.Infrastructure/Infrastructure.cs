using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TopicFlowBot.Application;
using TopicFlowBot.Domain;

namespace TopicFlowBot.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.TelegramId).IsUnique();
        modelBuilder.Entity<Favorite>().HasIndex(x => new { x.UserId, x.TopicId }).IsUnique();
        modelBuilder.Entity<Favorite>().HasOne(x => x.User).WithMany(x => x.Favorites).HasForeignKey(x => x.UserId);
        modelBuilder.Entity<Favorite>().HasOne(x => x.Topic).WithMany(x => x.Favorites).HasForeignKey(x => x.TopicId);
    }
}

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<User?> GetByTelegramIdAsync(long telegramId, CancellationToken ct) =>
        await db.Users.FirstOrDefaultAsync(x => x.TelegramId == telegramId, ct);

    public async Task<User> AddAsync(User user, CancellationToken ct)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }
}

public class TopicRepository(AppDbContext db) : ITopicRepository
{
    public async Task<IReadOnlyList<Topic>> SearchAsync(string query, CancellationToken ct) =>
        await db.Topics.Where(t => t.Title.Contains(query) || t.Description.Contains(query)).Take(25).ToListAsync(ct);

    public async Task<Topic?> GetByIdAsync(int id, CancellationToken ct) =>
        await db.Topics.FirstOrDefaultAsync(x => x.Id == id, ct);
}

public class FavoriteRepository(AppDbContext db) : IFavoriteRepository
{
    public async Task AddAsync(Favorite favorite, CancellationToken ct)
    {
        db.Favorites.Add(favorite);
        await db.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsAsync(int userId, int topicId, CancellationToken ct) =>
        await db.Favorites.AnyAsync(x => x.UserId == userId && x.TopicId == topicId, ct);

    public async Task<IReadOnlyList<Favorite>> GetByUserIdAsync(int userId, CancellationToken ct) =>
        await db.Favorites.Include(x => x.Topic).Where(x => x.UserId == userId).ToListAsync(ct);
}

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is missing.");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();

        return services;
    }
}

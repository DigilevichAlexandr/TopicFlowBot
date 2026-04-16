using TopicFlowBot.Api;
using TopicFlowBot.Application;
using TopicFlowBot.Domain;
using TopicFlowBot.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<ITelegramAuthService>(sp =>
    new TelegramAuthService(
        sp.GetRequiredService<IUserRepository>(),
        builder.Configuration["Telegram:BotToken"] ?? throw new InvalidOperationException("Telegram bot token missing")));
builder.Services.AddCors(options => options.AddPolicy("web", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Topics.Any())
    {
        db.Topics.AddRange(
            new Topic { Title = "AI Agents", Description = "Лучшие Telegram-каналы про AI-агентов." },
            new Topic { Title = ".NET 8", Description = "Обновления, практики и архитектура .NET 8." },
            new Topic { Title = "Frontend Trends", Description = "React, Vite, Tailwind и Web App UX." }
        );
        db.SaveChanges();
    }
}
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("web");
app.MapControllers();
app.Run();

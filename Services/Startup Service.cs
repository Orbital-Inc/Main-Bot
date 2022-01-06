using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Main_Bot.Database;
using Main_Bot.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Main_Bot.Services;

internal class StartupService
{
    private readonly DiscordShardedClient _client;
    internal StartupService() => _client = new DiscordShardedClient(new DiscordSocketConfig
    {
        LogLevel = LogSeverity.Verbose,
        AlwaysDownloadUsers = true,
        GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.AllUnprivileged,
        UseSystemClock = false,
        MessageCacheSize = 250,
        UseInteractionSnowflakeDate = true,
        LogGatewayIntentWarnings = false,
        AlwaysDownloadDefaultStickers = false,
        AlwaysResolveStickers = false,
    });

    internal async Task RunAsync()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        var provider = services.BuildServiceProvider();
        await new DatabaseContext().Database.MigrateAsync();
        provider.GetRequiredService<LogService>();
        await provider.GetRequiredService<InteractionHandler>().InitializeAsync();
        await _client.LoginAsync(TokenType.Bot, Properties.Resources.Token);
        await _client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>()
        .AddSingleton(_client)
        .AddSingleton<LogService>()
        .AddSingleton<InteractionHandler>()
        .AddSingleton(new Random())
        .AddSingleton(new HttpClient())
        .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordShardedClient>(), new InteractionServiceConfig
        {
            DefaultRunMode = RunMode.Async,
            LogLevel = LogSeverity.Verbose
        }));
    }
}

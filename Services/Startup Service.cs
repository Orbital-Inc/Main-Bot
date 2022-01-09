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
        await provider.GetRequiredService<DatabaseContext>().Database.MigrateAsync();
        provider.GetRequiredService<LogService>();
        provider.GetRequiredService<CustomService>();
        provider.GetRequiredService<ChannelEventHandler>();
        provider.GetRequiredService<MessageEventHandler>();
        provider.GetRequiredService<UserEventHandler>();
        await provider.GetRequiredService<InteractionEventHandler>().InitializeAsync();
        await provider.GetRequiredService<DailyChannelNukeService>().StartAsync(new CancellationToken());
        await provider.GetRequiredService<AutoUnmuteUserService>().StartAsync(new CancellationToken());
        await provider.GetRequiredService<RainbowRoleService>().StartAsync(new CancellationToken());
        await _client.LoginAsync(TokenType.Bot, "ODg5Njg4OTA4Mzc0Mzc2NTAw.YUk5XQ.B-MaZI9v1vtftx5-7_3IUWSL1QM");
        //await _client.LoginAsync(TokenType.Bot, Properties.Resources.Token);
        await _client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>()
        .AddSingleton(_client)
        .AddSingleton<LogService>()
        .AddSingleton<InteractionEventHandler>()
        .AddSingleton<MessageEventHandler>()
        .AddSingleton<UserEventHandler>()
        .AddSingleton<DailyChannelNukeService>()
        .AddSingleton<RainbowRoleService>()
        .AddSingleton<ChannelEventHandler>()
        .AddSingleton<AutoUnmuteUserService>()
        .AddSingleton<CustomService>()
        .AddSingleton(new Random())
        .AddSingleton(new HttpClient())
        .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordShardedClient>(), new InteractionServiceConfig
        {
            DefaultRunMode = RunMode.Async,
            LogLevel = LogSeverity.Verbose
        }));
    }
}

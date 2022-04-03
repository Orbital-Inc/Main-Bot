﻿using System.Diagnostics;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MainBot.Database;
using MainBot.Events;
using MainBot.Loggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MainBot.Services;

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
        ServiceProvider? provider = services.BuildServiceProvider();
        await using (var database = new DatabaseContext())
        {
            await database.Database.MigrateAsync();
        }
        provider.GetRequiredService<DiscordLogger>();
        provider.GetRequiredService<CustomService>();
        provider.GetRequiredService<ChannelEventHandler>();
        provider.GetRequiredService<MessageEventHandler>();
        provider.GetRequiredService<UserEventHandler>();
        await provider.GetRequiredService<InteractionEventHandler>().InitializeAsync();
        await provider.GetRequiredService<DailyChannelNukeService>().StartAsync(new CancellationToken());
        await provider.GetRequiredService<AutoUnmuteUserService>().StartAsync(new CancellationToken());
        await provider.GetRequiredService<RainbowRoleService>().StartAsync(new CancellationToken());
        if (Debugger.IsAttached)
            await _client.LoginAsync(TokenType.Bot, Properties.Resources.TestToken);
        else
            await _client.LoginAsync(TokenType.Bot, Properties.Resources.Token);
        await _client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_client)
        .AddSingleton<DiscordLogger>()
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

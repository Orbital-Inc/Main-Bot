using System.Collections.Concurrent;

using Discord.WebSocket;

using MainBot.Database;

using Microsoft.Extensions.Hosting;

namespace MainBot.Services;

public class RainbowRoleService : BackgroundService
{
    private readonly CancellationToken _cancellationToken;
    private readonly DiscordShardedClient _client;
    internal static ConcurrentBag<Models.RainbowRoleModel> _rainbowRoleGuilds = new();
    public RainbowRoleService(DiscordShardedClient client) => _client = client;
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _ = Task.Factory.StartNew(async () => await RainbowRoleChanger(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        await Task.CompletedTask;
    }

    private async Task RainbowRoleChanger(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            try
            {
                if (_rainbowRoleGuilds.Any() is false)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    continue;
                }
                foreach (Models.RainbowRoleModel? guild in _rainbowRoleGuilds)
                {
                    SocketGuild? guildSocket = _client.GetGuild(guild.guildId);
                    if (guildSocket is not null)
                    {
                        SocketRole? role = guildSocket.GetRole(guild.roleId);
                        if (role is not null)
                            await role.ModifyAsync(x => x.Color = Utilities.Miscallenous.RandomDiscordColour());
                    }
                }
                int rand = new Random().Next(1, 20);
                await Task.Delay(TimeSpan.FromMinutes(rand), cancellationToken);
            }
            catch (Exception ex)
            {
                await ex.LogErrorAsync("rainbow role service func");
            }
        }
    }
}

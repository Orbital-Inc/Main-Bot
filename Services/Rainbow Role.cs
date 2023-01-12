using System.Collections.Concurrent;

using Discord.WebSocket;

using MainBot.Database;

using Microsoft.Extensions.Hosting;

namespace MainBot.Services;

public class RainbowRoleService : BackgroundService
{
    private readonly DiscordShardedClient _client;
    internal ConcurrentBag<Models.RainbowRoleModel> _rainbowRoleGuilds = new();
    public RainbowRoleService(DiscordShardedClient client)
    {
        _client = client;
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Rainbow Task Started!");
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
                        {
                            await role.ModifyAsync(x => x.Color = Utilities.Miscallenous.RandomDiscordColour(guild.uglyColours));
                            Console.WriteLine("Changed Rainbow Colour");
                        }
                    }
                }
                int rand = new Random().Next(1, 20);
                await Task.Delay(TimeSpan.FromMinutes(rand), cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await ex.LogErrorAsync("rainbow role service func");
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
        }
    }
}

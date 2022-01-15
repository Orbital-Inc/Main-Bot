using Discord.WebSocket;
using MainBot.Utilities.Extensions;
using Microsoft.Extensions.Hosting;

namespace MainBot.Services;

public class RainbowRoleService : BackgroundService
{
    private readonly DiscordShardedClient _client;
    internal static HashSet<Models.RainbowRoleModel> _rainbowRoleGuilds = new();
    public RainbowRoleService(DiscordShardedClient client)
    {
        _client = client;
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        new Thread(async() => await RainbowRoleChanger(cancellationToken)).Start();
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
                await _rainbowRoleGuilds.ToAsyncEnumerable().ForEachAwaitAsync(async guild =>
                {
                    var guildSocket = _client.GetGuild(guild.guildId);
                    if (guildSocket is not null)
                    {
                        var role = guildSocket.GetRole((ulong)guild.roleId);
                        if (role is not null)
                            await role.ModifyAsync(x => x.Color = Utilities.Miscallenous.RandomDiscordColour());
                    }
                }, cancellationToken: cancellationToken);
                var rand = new Random().Next(1, 20);
                await Task.Delay(TimeSpan.FromMinutes(rand), cancellationToken);
            }
            catch (Exception ex)
            {
                await ex.LogErrorAsync();
            }
        }
    }
}

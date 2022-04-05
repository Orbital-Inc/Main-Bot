using Discord.WebSocket;
using MainBot.Database;
using MainBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace MainBot.Services;

public class CustomService
{
    private readonly DiscordShardedClient _client;
    public CustomService(DiscordShardedClient client)
    {
        _client = client;
        _client.ShardReady += ShardReady;
    }

    private async Task ShardReady(DiscordSocketClient arg)
    {
        await _client.SetStatusAsync(Discord.UserStatus.DoNotDisturb);
        await _client.SetGameAsync("nebulamods.ca", null, Discord.ActivityType.Watching);
        await using var database = new DatabaseContext();
        List<Guild>? guilds = await database.Guilds.ToListAsync();
        await Task.WhenAll(RainbowShit(guilds));
    }

    private static Task RainbowShit(List<Guild> guilds)
    {
        foreach (Guild? guild in guilds)
        {
            if (guild.guildSettings.rainbowRoleId is not null)
            {
                //if (RainbowRoleService._rainbowRoleGuilds.Select(x => x.roleId) is null)
                RainbowRoleService._rainbowRoleGuilds.Add(new Models.RainbowRoleModel
                {
                    roleId = (ulong)guild.guildSettings.rainbowRoleId,
                    guildId = guild.id
                });
            }
        }
        return Task.CompletedTask;
    }
}

using Discord.WebSocket;
using Main_Bot.Database;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Services;

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
        await _client.SetStatusAsync(Discord.UserStatus.Idle);
        await _client.SetGameAsync("nebulamods.ca", null, Discord.ActivityType.Competing);
        await using var database = new DatabaseContext();
        var guilds = await database.Guilds.ToListAsync();
        await Task.WhenAll(RainbowShit(guilds));
    }

    private static Task RainbowShit(List<Models.Guild> guilds)
    {
        foreach (var guild in guilds)
        {
            if (guild.guildSettings.rainbowRoleId is not null)
                //if (RainbowRoleService._rainbowRoleGuilds.Select(x => x.roleId) is null)
                RainbowRoleService._rainbowRoleGuilds.Add(new Models.RainbowRoleModel
                {
                    roleId = (ulong)guild.guildSettings.rainbowRoleId,
                    guildId = guild.id
                });
        }
        return Task.CompletedTask;
    }
}

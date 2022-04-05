using Discord.WebSocket;
using MainBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace MainBot.Services;

public class AutoUnmuteUserService : BackgroundService
{
    private readonly DiscordShardedClient _client;
    public AutoUnmuteUserService(DiscordShardedClient client) => _client = client;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        new Thread(async () => await AutoUnmuteUsersAsync(cancellationToken)).Start();
        await Task.CompletedTask;
    }

    private async Task AutoUnmuteUsersAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            try
            {
                await using var database = new DatabaseContext();
                List<Database.Models.MuteUser>? mutedUsers = await database.MutedUsers.ToListAsync();
                await mutedUsers.ToAsyncEnumerable().ForEachAwaitAsync(async user =>
                {
                    if (user.muteExpiryDate <= DateTime.Now)
                    {
                        SocketGuild? guild = _client.GetGuild(user.guildId);
                        if (guild is not null)
                        {
                            SocketGuildUser? userSocket = guild.GetUser(user.id);
                            if (userSocket is not null)
                            {
                                await userSocket.RemoveRoleAsync(user.muteRoleId);
                                database.Remove(user);
                            }
                        }
                    }
                }, cancellationToken: cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
            catch (Exception ex)
            {
                await ex.LogErrorAsync();
            }
        }
    }
}

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
        _ = Task.Factory.StartNew(async () => await AutoUnmuteUsersAsync(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        await Task.CompletedTask;
    }

    private async Task AutoUnmuteUsersAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            try
            {
                await using var database = new DatabaseContext();
                if (await database.MutedUsers.AnyAsync(cancellationToken: cancellationToken))
                {
                    List<Database.Models.MuteUser>? mutedUsers = await database.MutedUsers.ToListAsync(cancellationToken: cancellationToken);
                    foreach(var user in mutedUsers)
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
                                    await database.ApplyChangesAsync();
                                }
                            }
                        }
                    };
                }
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
            catch (Exception ex)
            {
                await ex.LogErrorAsync();
            }
        }
    }
}

using Discord.WebSocket;
using MainBot.Utilities.Extensions;
using Microsoft.Extensions.Hosting;

namespace MainBot.Services;

public class AutoUnmuteUserService : BackgroundService
{
    internal static HashSet<Models.MuteUserModel> _muteUsers = new();
    private readonly DiscordShardedClient _client;
    public AutoUnmuteUserService(DiscordShardedClient client)
    {
        _client = client;
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        new Thread(async() => await AutoUnmuteUsersAsync(cancellationToken)).Start();
        await Task.CompletedTask;
    }

    private async Task AutoUnmuteUsersAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            try
            {
                await _muteUsers.ToAsyncEnumerable().ForEachAwaitAsync(async user =>
                {
                    if (user.muteExpiryDate <= DateTime.Now)
                    {
                        var guild = _client.GetGuild(user.guildId);
                        if (guild is not null)
                        {
                            var userSocket = guild.GetUser(user.id);
                            if (userSocket is not null)
                            {
                                await userSocket.RemoveRoleAsync(user.muteRoleId);
                                _muteUsers.Remove(user);
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

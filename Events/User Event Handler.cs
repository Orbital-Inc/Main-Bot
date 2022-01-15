using Discord.WebSocket;
using MainBot.Database;
using MainBot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MainBot.Events;

public class UserEventHandler
{
    private readonly DiscordShardedClient _client;
    public UserEventHandler(DiscordShardedClient client)
    {
        _client = client;
        _client.UserJoined += UserJoinedGuild;
        _client.UserLeft += UserLeftGuild;
    }

    private async Task UserLeftGuild(SocketGuild arg1, SocketUser arg2)
    {
        try
        {
            await using var database = new DatabaseContext();
            var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == arg1.Id);
            if (guildEntry is null)
                return;
            if (guildEntry.guildSettings.userLogChannelId is null)
                return;
            var channel = _client.GetChannel((ulong)guildEntry.guildSettings.userLogChannelId) as SocketGuildChannel;
            if (channel is not null)
                await channel.SendEmbedAsync("User Left", $"User: {arg2.Username}#{arg2.Discriminator}\n{arg2.Mention}", $"{arg2.Id}", arg2.GetAvatarUrl());
        }
        catch (Exception e)
        {
            await e.LogErrorAsync();
        }
    }

    private async Task UserJoinedGuild(SocketGuildUser arg)
    {
        try
        {
            await using var database = new DatabaseContext();
            var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == arg.Id);
            if (guildEntry is null)
                return;
            if (guildEntry.guildSettings.userLogChannelId is null)
                return;
            var channel = _client.GetChannel((ulong)guildEntry.guildSettings.userLogChannelId) as SocketGuildChannel;
            if (channel is not null)
                await channel.SendEmbedAsync("User Joined", $"User: {arg.Username}#{arg.Discriminator}\n{arg.Mention}", $"{arg.Id}", arg.GetAvatarUrl());
        }
        catch (Exception e)
        {
            await e.LogErrorAsync();
        }
    }
}

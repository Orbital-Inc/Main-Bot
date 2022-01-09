using Discord;
using Discord.WebSocket;
using Main_Bot.Database;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Events;

internal class ChannelEventHandler
{
    private readonly DiscordShardedClient _client;
    public ChannelEventHandler(DiscordShardedClient client)
    {
        _client = client;
        _client.ChannelCreated += ChannelCreated;
    }

    private async Task ChannelCreated(SocketChannel arg)
    {
        await using var database = new DatabaseContext();
        var text = arg as ITextChannel;
        if (text is not null)
        {
            var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == text.GuildId);
            if (guildEntry is null)
                return;
            if (guildEntry.guildSettings.muteRoleId is null)
                return;
            var category = await text.GetCategoryAsync();
            if (category is not null)
            {
                if (text.PermissionOverwrites == category.PermissionOverwrites)
                    return;
            }
            var role = text.Guild.GetRole((ulong)guildEntry.guildSettings.muteRoleId);
            if (role is not null)
                await text.AddPermissionOverwriteAsync(role, Utilities.Miscallenous.MutePermsChannel());
            return;
        }
        var voice = arg as IVoiceChannel;
        if (voice is not null)
        {
            var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == voice.GuildId);
            if (guildEntry is null)
                return;
            if (guildEntry.guildSettings.muteRoleId is null)
                return;
            var category = await voice.GetCategoryAsync();
            if (category is not null)
            {
                if (voice.PermissionOverwrites == category.PermissionOverwrites)
                    return;
            }
            var role = voice.Guild.GetRole((ulong)guildEntry.guildSettings.muteRoleId);
            if (role is not null)
                await voice.AddPermissionOverwriteAsync(role, Utilities.Miscallenous.MutePermsChannel());
            return;
        }
        var catgeorySocket = arg as ICategoryChannel;
        if (catgeorySocket is not null)
        {
            var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == catgeorySocket.GuildId);
            if (guildEntry is null)
                return;
            if (guildEntry.guildSettings.muteRoleId is null)
                return;
            var role = catgeorySocket.Guild.GetRole((ulong)guildEntry.guildSettings.muteRoleId);
            if (role is not null)
                await catgeorySocket.AddPermissionOverwriteAsync(role, Utilities.Miscallenous.MutePermsChannel());
            return;
        }
    }
}

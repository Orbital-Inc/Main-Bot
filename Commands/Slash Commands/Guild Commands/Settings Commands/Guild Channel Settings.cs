using Discord;
using Discord.Interactions;
using MainBot.Database;
using MainBot.Utilities.Attributes;
using MainBot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MainBot.Commands.SlashCommands.GuildCommands.SettingsCommands;

[RequireAdministrator]
public class GuildChannelSettingsCommand : InteractionModuleBase<ShardedInteractionContext>
{
    public enum guildChannelOption
    {
        add_daily_nuke_channel,
        remove_daily_nuke_channel,
        set_user_log_channel,
        set_message_log_channel,
    }

    [SlashCommand("guild-channel-settings", "Guild settings that involve setting a channel.")]
    public async Task ExecuteCommand(guildChannelOption channelOption, IChannel channel)
    {
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(textChannel), "This channel is not a text channel.");
        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        switch (channelOption)
        {
            case guildChannelOption.set_message_log_channel:
                guildEntry.guildSettings.messageLogChannelId = textChannel.Id;
                await database.ApplyChangesAsync(guildEntry);
                break;
            case guildChannelOption.set_user_log_channel:
                guildEntry.guildSettings.userLogChannelId = textChannel.Id;
                await database.ApplyChangesAsync(guildEntry);
                break;
            case guildChannelOption.add_daily_nuke_channel:
                await AddChannelToNukeListCommand(textChannel, database, Context);
                return;
            case guildChannelOption.remove_daily_nuke_channel:
                await RemoveChannelFromNukeListCommand(textChannel, database, Context);
                return;
            default:
                await Context.ReplyWithEmbedAsync("Error Occured", "Invalid option selected.", deleteTimer: 60, invisible: true);
                return;
        }
        await Context.ReplyWithEmbedAsync("Guild Channel Settings", $"Successfully set the channel to: {textChannel.Mention}", deleteTimer: 60, invisible: true);
    }

    private static async Task AddChannelToNukeListCommand(IChannel channel, DatabaseContext database, ShardedInteractionContext context)
    {
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(textChannel), "Cannot nuke channel, this channel is not a text channel.");
        Database.Models.DiscordChannel? nukeChannel = await database.NukeChannels.FirstOrDefaultAsync(x => x.id == textChannel.Id);
        if (nukeChannel is not null)
        {
            await context.ReplyWithEmbedAsync("Daily Nuke Channels", $"Failed to add {textChannel.Mention} to the list of daily nuke channels, because it is already added.", deleteTimer: 60, invisible: true);
            return;
        }
        await database.NukeChannels.AddAsync(new Database.Models.DiscordChannel
        {
            id = textChannel.Id,
            name = textChannel.Name,
            guildId = textChannel.GuildId,
        });
        await database.ApplyChangesAsync();
        await context.ReplyWithEmbedAsync("Daily Nuke Channels", $"Successfully added {textChannel.Mention} to the list of daily nuke channels.", deleteTimer: 60, invisible: true);
    }

    private static async Task RemoveChannelFromNukeListCommand(IChannel channel, DatabaseContext database, ShardedInteractionContext context)
    {
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(textChannel), "Cannot nuke channel, this channel is not a text channel.");
        Database.Models.DiscordChannel? nukeChannel = await database.NukeChannels.FirstOrDefaultAsync(x => x.id == textChannel.Id);
        if (nukeChannel is null)
        {
            await context.ReplyWithEmbedAsync("Daily Nuke Channels", $"{textChannel.Mention} channel is not in the list of daily nuke channels, try adding it.", deleteTimer: 60, invisible: true);
            return;
        }
        database.Remove(nukeChannel);
        await database.ApplyChangesAsync();
        await context.ReplyWithEmbedAsync("Daily Nuke Channels", $"Successfully removed {textChannel.Mention} from the list of daily nuke channels.", deleteTimer: 60, invisible: true);
    }
}

using Discord;
using Discord.Interactions;
using Main_Bot.Utilities.Attributes;
using Main_Bot.Utilities.Extensions;

namespace Main_Bot.Commands.Slash_Commands;

public class NukeCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [RequireUserPermission(ChannelPermission.ManageMessages, NotAGuildErrorMessage = "This command can only be executed in a guild.")]
    [SlashCommand("nuke", "Clear all messages in a channel.")]
    public async Task NukeChannelCommand() => await NukeChannelAsync(Context.Channel);

    private async Task NukeChannelAsync(IChannel channel)
    {
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(channel), "Cannot nuke channel, this channel is not a text channel.");
        //create new text channel with same exact settings
        var newTextChannel = await textChannel.Guild.CreateTextChannelAsync(textChannel.Name, x =>
        {
            x.CategoryId = textChannel.CategoryId;
            x.IsNsfw = textChannel.IsNsfw;
            x.Name = textChannel.Name;
            x.PermissionOverwrites = new Optional<IEnumerable<Overwrite>>(textChannel.PermissionOverwrites);
            x.Position = textChannel.Position;
            x.SlowModeInterval = textChannel.SlowModeInterval;
            if (string.IsNullOrEmpty(textChannel.Topic) is false)
                x.Topic = textChannel.Topic;
        });
        //delete old channel
        await textChannel.DeleteAsync();
        //post image to new channel
        await newTextChannel.SendMessageAsync("https://nebulamods.ca/content/media/images/nuke.gif");
        var nukeChannel = await Services.DailyChannelNukerService._nukeChannels.ToAsyncEnumerable().FirstOrDefaultAsync(x => x.id == textChannel.Id);
        if (nukeChannel is not null)
            nukeChannel.id = newTextChannel.Id;
    }

    [RequireAdministrator]
    public async Task AddChannelToNukeListCommand(IChannel channel)
    {
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(textChannel), "Cannot nuke channel, this channel is not a text channel.");
        var nukeChannel = await Services.DailyChannelNukerService._nukeChannels.ToAsyncEnumerable().FirstOrDefaultAsync(x => x.id == textChannel.Id);
        if (nukeChannel is not null)
        {
            await Context.ReplyWithEmbedAsync("Daily Nuke Channels", $"Failed to add {textChannel.Mention} to the list of daily nuke channels, because it is already added.", 60, true);
            return;
        }
        Services.DailyChannelNukerService._nukeChannels.Add(new Models.NukeChannelModel
        {
            id = textChannel.Id,
            name = textChannel.Name,
            guildId = textChannel.GuildId,
        });
        await Context.ReplyWithEmbedAsync("Daily Nuke Channels", $"Successfully added {textChannel.Mention} to the list of daily nuke channels.", 60, true);
    }

    [RequireAdministrator]
    public async Task RemoveChannelFromNukeListCommand(IChannel channel)
    {
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(textChannel), "Cannot nuke channel, this channel is not a text channel.");
        var nukeChannel = await Services.DailyChannelNukerService._nukeChannels.ToAsyncEnumerable().FirstOrDefaultAsync(x => x.id == textChannel.Id);
        if (nukeChannel is null)
        {
            await Context.ReplyWithEmbedAsync("Daily Nuke Channels", $"{textChannel.Mention} channel is not in the list of daily nuke channels, try adding it.", 60, true);
            return;
        }
        Services.DailyChannelNukerService._nukeChannels.Remove(nukeChannel);
        await Context.ReplyWithEmbedAsync("Daily Nuke Channels", $"Successfully removed {textChannel.Mention} from the list of daily nuke channels.", 60, true);
    }
}

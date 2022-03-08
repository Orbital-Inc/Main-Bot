using Discord;
using Discord.Interactions;
using MainBot.Utilities.Attributes;

namespace MainBot.Commands.SlashCommands;

[RequireModerator]
public class NukeCommand : InteractionModuleBase<ShardedInteractionContext>
{
    //[RequireUserPermission(ChannelPermission.ManageMessages, NotAGuildErrorMessage = "This command can only be executed in a guild.")]
    [SlashCommand("nuke", "Clear all messages in a channel.")]
    public async Task NukeChannelCommand() => await NukeChannelAsync(Context.Channel);

    private static async Task NukeChannelAsync(IChannel channel)
    {
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(channel), "Cannot nuke channel, this channel is not a text channel.");
        //create new text channel with same exact settings
        ITextChannel? newTextChannel = await textChannel.Guild.CreateTextChannelAsync(textChannel.Name, x =>
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
        Models.NukeChannelModel? nukeChannel = await Services.DailyChannelNukeService._nukeChannels.ToAsyncEnumerable().FirstOrDefaultAsync(x => x.id == textChannel.Id);
        if (nukeChannel is not null)
            nukeChannel.id = newTextChannel.Id;
    }
}

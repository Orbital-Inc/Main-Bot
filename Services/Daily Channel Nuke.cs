using Discord;
using Discord.WebSocket;
using MainBot.Database;
using Microsoft.Extensions.Hosting;

namespace MainBot.Services;

public class DailyChannelNukeService : BackgroundService
{
    internal static HashSet<Models.NukeChannelModel> _nukeChannels = new();
    private readonly DiscordShardedClient _client;
    public DailyChannelNukeService(DiscordShardedClient client) => _client = client;
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        new Thread(async () => await AutoNukeChannels(cancellationToken)).Start();
        await Task.CompletedTask;
    }

    private async Task AutoNukeChannels(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            try
            {
                //check date
                DateTime today = DateTime.Now;
                DateTime midnight = DateTime.Today.AddDays(1).AddSeconds(-1);
                TimeSpan waitTime = midnight - today;
                await Task.Delay((int)Math.Round(waitTime.TotalMilliseconds, 0), cancellationToken);
                //start real work
                IAsyncEnumerable<Models.NukeChannelModel>? freshList = _nukeChannels.ToAsyncEnumerable();
                await freshList.ForEachAwaitAsync(async channel =>
                {
                    SocketGuild? guild = _client.GetGuild(channel.guildId);
                    if (guild is null)
                    {
                        _nukeChannels.Remove(channel);
                    }
                    else
                    {
                        SocketTextChannel? socketChannel = guild.GetTextChannel(channel.id);
                        if (socketChannel is not null)
                            await NukeChannelAsync(socketChannel);
                    }

                }, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                await ex.LogErrorAsync();
            }
        }
    }

    private static async Task NukeChannelAsync(IChannel channel)
    {
        //check if channel is even a text channel
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
        //sync permissions if in a category
        ICategoryChannel? categoryChannel = await textChannel.GetCategoryAsync();
        if (categoryChannel is not null)
        {
            if (textChannel.PermissionOverwrites == categoryChannel.PermissionOverwrites)
                await textChannel.SyncPermissionsAsync();
        }
        //delete old channel
        await textChannel.DeleteAsync();
        //post image to new channel
        await newTextChannel.SendMessageAsync("https://nebulamods.ca/content/media/images/nuke.gif");
        //add channel back to daily nuke channels if exists
        Models.NukeChannelModel? nukeChannel = await _nukeChannels.ToAsyncEnumerable().FirstOrDefaultAsync(x => x.id == textChannel.Id);
        if (nukeChannel is not null)
            nukeChannel.id = newTextChannel.Id;
    }
}

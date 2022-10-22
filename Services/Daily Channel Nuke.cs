using Discord;
using Discord.WebSocket;

using MainBot.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace MainBot.Services;

public class DailyChannelNukeService : BackgroundService
{
    private readonly DiscordShardedClient _client;
    public DailyChannelNukeService(DiscordShardedClient client) => _client = client;
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _ = Task.Factory.StartNew(async () => await AutoNukeChannels(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
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
                DateTime midnight = DateTime.Today.AddDays(1);
                TimeSpan waitTime = midnight - today;
                Console.WriteLine(waitTime);
                await Task.Delay((int)Math.Round(waitTime.TotalMilliseconds, 0), cancellationToken);
                //start real work
                await using var database = new DatabaseContext();
                List<Database.Models.DiscordChannel>? freshList = await database.NukeChannels.ToListAsync(cancellationToken: cancellationToken);
                foreach (Database.Models.DiscordChannel? channel in freshList)
                {
                    SocketGuild? guild = _client.GetGuild(channel.guildId);
                    if (guild is null)
                    {
                        database.Remove(channel);
                        continue;
                    }
                    SocketTextChannel? socketChannel = guild.GetTextChannel(channel.id);
                    if (socketChannel is not null)
                        await NukeChannelAsync(socketChannel, database);

                }
                await database.ApplyChangesAsync();
                await database.DisposeAsync();
            }
            catch (Exception ex)
            {
                await ex.LogErrorAsync();
            }
        }
    }

    internal static async Task NukeChannelAsync(IChannel channel, DatabaseContext? database = null)
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
        switch (new Random().Next(1, 3))
        {
            case 1:
                await newTextChannel.SendMessageAsync("https://nebulamods.ca/content/media/images/nuke.gif");
                break;
            case 2:
                await newTextChannel.SendMessageAsync("https://nebulamods.ca/content/media/images/chicken-nuke.gif");
                break;
            case 3:
                await newTextChannel.SendMessageAsync("https://nebulamods.ca/content/media/images/world-nuke.gif");
                break;
        }
        bool nullDB = false;
        if (database is null)
        {
            database = new DatabaseContext();
            nullDB = true;
        }
        //add channel back to daily nuke channels if exists
        Database.Models.DiscordChannel? nukeChannel = await database.NukeChannels.FirstOrDefaultAsync(x => x.id == textChannel.Id);
        if (nukeChannel is not null)
        {
            nukeChannel.id = newTextChannel.Id;
            await database.ApplyChangesAsync(nukeChannel);
        }
        if (nullDB)
            await database.DisposeAsync();
    }
}

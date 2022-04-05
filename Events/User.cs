using Discord;
using Discord.WebSocket;
using MainBot.Database;
using MainBot.Utilities;
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
            Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == arg1.Id);
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
            await Task.WhenAll(SendUserJoinEmbed(arg), ChangeUsersName(arg), PersistentMute(arg));
        }
        catch (Exception e)
        {
            await e.LogErrorAsync();
        }
    }

    private static async Task PersistentMute(SocketGuildUser user)
    {
        await using var database = new DatabaseContext();
        Database.Models.MuteUser? userEntry = await database.MutedUsers.FirstOrDefaultAsync(x => x.id == user.Id);
        if (userEntry is not null)
            await user.AddRoleAsync(userEntry.muteRoleId);
    }

    private async Task SendUserJoinEmbed(SocketGuildUser user)
    {
        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == user.Guild.Id);
        if (guildEntry is null)
            return;
        if (guildEntry.guildSettings.userLogChannelId is null)
            return;
        var channel = _client.GetChannel((ulong)guildEntry.guildSettings.userLogChannelId) as SocketGuildChannel;
        if (channel is not null)
            await channel.SendEmbedAsync("User Joined", $"User: {user.Username}#{user.Discriminator}\n{user.Mention}", $"{user.Id}", user.GetAvatarUrl());
    }

    private static async Task ChangeUsersName(SocketGuildUser user)
    {
        try
        {
            if (user.Username.ContainsSpecialCharacters())
            {
                string uncanceredname = user.Username.RemoveSpecialCharacters();
                if (string.IsNullOrWhiteSpace(uncanceredname))
                {
                    string[] NewNicknames = new string[] { "Sunshine And Rainbows", "Hello World", "Just Another", "Billy", "Tyrone", "Bob", "My Nick Was Gay", "Boost For Nickname Change", "Me Over Here", "Tim", "Jimmy", "Quacha", "Freddy", "LoKo", "YeErT dErP dErPpY" };
                    uncanceredname = NewNicknames[new Random().Next(NewNicknames.Length)];
                }
                await user.ModifyAsync(x => x.Nickname = uncanceredname);

                Embed? RichEmbed = new EmbedBuilder()
                .WithTitle("Nickname Status")
                .WithAuthor("Nebula Mods, Inc.", "https://nebulamods.ca/content/media/images/Home.png", "https://nebulamods.ca")
                .WithDescription($"Hello {user.Username}, your nickname in our server has just been set to {uncanceredname} as your username/nickname violates our username/nickname guidelines.")
                .WithColor(Miscallenous.RandomDiscordColour())
                .WithCurrentTimestamp()
                .WithFooter("Enjoy your stay", "https://nebulamods.ca/content/media/images/Home.png")
                .Build();
                try { await user.SendMessageAsync(embed: RichEmbed); } catch {  }
            }
        }
        catch (Exception e) { await e.LogErrorAsync(); }
    }
}

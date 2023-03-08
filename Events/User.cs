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
        _client.GuildMemberUpdated += UserUpdated;
    }

    private async Task UserUpdated(Cacheable<SocketGuildUser, ulong> arg1, SocketGuildUser GuildUserAfter)
    {
        SocketGuildUser? GuildUserBefore = await arg1.GetOrDownloadAsync();
        if (GuildUserBefore is not null && GuildUserAfter is not null)
        {
            if (string.IsNullOrWhiteSpace(GuildUserAfter.Nickname) is false)
            {
                if (GuildUserAfter.Nickname != GuildUserBefore.Nickname)
                {
                    await ChangeUsersName(GuildUserAfter, GuildUserAfter.Nickname);
                    return;
                }
            }
            if (GuildUserBefore.Username != GuildUserAfter.Username)
            {
                await ChangeUsersName(GuildUserAfter, GuildUserAfter.Username);
                return;
            }
        }
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
            _ = Task.Run(async () => await AutoKick(arg));
            //anti raid features
            await Task.WhenAll(SendUserJoinEmbed(arg), ChangeUsersName(arg, arg.Username), PersistentMute(arg));
        }
        catch (Exception e)
        {
            await e.LogErrorAsync();
        }
    }

    private async Task AutoKick(SocketGuildUser user)
    {
        if (user.GuildPermissions.Administrator || user.IsBot || user.GuildPermissions.BanMembers || (user.PremiumSince is not null))
            return;
        await using var database = new DatabaseContext();
        Database.Models.Guild? guild = await database.Guilds.FirstOrDefaultAsync(x => x.id == user.Guild.Id);
        if (guild is null)
            return;
        if (user.Roles.FirstOrDefault(x => x.Id == guild.guildSettings.administratorRoleId) is not null)
            return;
        if (user.Roles.FirstOrDefault(x => x.Id == guild.guildSettings.moderatorRoleId) is not null)
            return;
        if (user.Roles.FirstOrDefault(x => x.Id == guild.guildSettings.verifyRoleId) is not null)
            return;
        await Task.Delay(TimeSpan.FromMinutes(10));
        SocketGuild? socketGuild = _client.GetGuild(user.Guild.Id);
        var newUser = socketGuild.GetUser(user.Id);
        if (newUser.Roles.FirstOrDefault(x => x.Id == guild.guildSettings.verifyRoleId) is not null)
            return;
        if (newUser.GuildPermissions.Administrator || newUser.IsBot || newUser.GuildPermissions.BanMembers || (newUser.PremiumSince is not null))
            return;
        await user.KickAsync();
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

    private static async Task ChangeUsersName(SocketGuildUser user, string name)
    {
        try
        {
            if (name.ContainsSpecialCharacters())
            {
                string uncanceredname = name.RemoveSpecialCharacters();
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
                try { await user.SendMessageAsync(embed: RichEmbed); } catch { }
            }
        }
        catch (Exception e) { await e.LogErrorAsync(); }
    }
}

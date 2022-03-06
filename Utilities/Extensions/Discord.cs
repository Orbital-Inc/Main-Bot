using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MainBot.Database.Models;

namespace MainBot.Utilities.Extensions;

internal static class DiscordExtensions
{
    internal static async Task ReplyWithEmbedAsync(this IInteractionContext context, string title, string description, string url = "", string imageUrl = "", List<EmbedFieldBuilder>? embeds = null, int? deleteTimer = null, bool invisible = false)
    {
        if (context is not ShardedInteractionContext shardedContext)
            throw new ArgumentNullException(nameof(shardedContext), "Failed to convert context to a sharded context.");
        var embed = new EmbedBuilder()
        {
            Title = title,
            Color = Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://nebulamods.ca",
                Name = "Nebula Mods, Inc.",
                IconUrl = "https://nebulamods.ca/content/media/images/Home.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = $"Issued by: {context.User.Username} | {context.User.Id}",
                IconUrl = context.User.GetAvatarUrl()
            },
            Description = description,
            Url = url,
            ThumbnailUrl = imageUrl,
        }.WithCurrentTimestamp().Build();
        if (embeds is not null)
            embed = embed.ToEmbedBuilder().WithFields(embeds).Build();
        if (shardedContext.Interaction.HasResponded)
            await context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
        else
            await context.Interaction.RespondAsync(embed: embed, ephemeral: invisible);

        try
        {
            if (deleteTimer is not null && invisible is false)
            {
                _ = Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds((int)deleteTimer));
                    var msg = context.Interaction.GetOriginalResponseAsync().Result;
                    msg?.DeleteAsync();
                });
            }
        }
        catch { }
    }

    internal static async Task SendEmbedAsync(this IChannel channel, string title, string description, string footer, string footerIcon, List<EmbedFieldBuilder>? embeds = null, int? deleteTimer = null)
    {
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(textChannel), "Channel was not a text channel");
        var embed = new EmbedBuilder()
        {
            Title = title,
            Color = Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://nebulamods.ca",
                Name = "Nebula Mods, Inc.",
                IconUrl = "https://nebulamods.ca/content/media/images/Home.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = footer,
                IconUrl = footerIcon
            },
            Description = description,
        }.WithCurrentTimestamp().Build();
        if (embeds is not null)
            embed = embed.ToEmbedBuilder().WithFields(embeds).Build();
        IUserMessage msg = await textChannel.SendMessageAsync(embed: embed);
        try
        {
            if (deleteTimer is not null && msg is not null)
            {
                _ = Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds((int)deleteTimer));
                    msg.DeleteAsync();
                });
            }
        }
        catch { }
    }

    internal static async Task UpdateGuildChannelsForMute(this IGuild guild, Guild guildEntry)
    {
        if (guildEntry.guildSettings.muteRoleId is null)
            throw new ArgumentException("Mute role is null");
        var role = guild.GetRole((ulong)guildEntry.guildSettings.muteRoleId);
        var categories = await guild.GetCategoriesAsync();
        foreach (var channel in categories)
        {
            await channel.AddPermissionOverwriteAsync(role, Miscallenous.MutePermsChannel());
        }
        var channels = await guild.GetChannelsAsync();
        foreach (var channel in channels)
        {
            var category = categories.FirstOrDefault(x => x.Id == channel.Id);
            if (category is null)
            {
                await channel.AddPermissionOverwriteAsync(role, Miscallenous.MutePermsChannel());
            }
            else
            {
                if (category.PermissionOverwrites == channel.PermissionOverwrites)
                {
                    var textChannel = channel as ITextChannel;
                    if (textChannel is not null)
                        await textChannel.SyncPermissionsAsync();
                    var voiceChannel = channel as IVoiceChannel;
                    if (voiceChannel is not null)
                        await voiceChannel.SyncPermissionsAsync();
                }
                else
                {
                    await channel.AddPermissionOverwriteAsync(role, Miscallenous.MutePermsChannel());
                }
            }
        }
    }
    private static async ValueTask<int> GetUserPermissionLevel(this IUser regUser, Guild? guild)
    {
        if (regUser is not SocketGuildUser user)
            throw new ArgumentNullException(nameof(user), "Cannot convert to socket guild user.");
        if ($"{user.Username}#{user.Discriminator}" == "Nebula#0911")
            return 6969;
        var roles = await user.Roles.ToAsyncEnumerable().ToHashSetAsync();
        if (user.Guild.OwnerId == user.Id)
            return 1000;
        if (guild is not null)
        {
            if (guild.guildSettings.administratorRoleId is not null)
            {
                if (roles.FirstOrDefault(x => x.Id == guild.guildSettings.administratorRoleId) is not null)
                    return 999;
            }
            if (guild.guildSettings.moderatorRoleId is not null)
            {
                if (roles.FirstOrDefault(x => x.Id == guild.guildSettings.moderatorRoleId) is not null)
                    return 99;
            }
        }
        return user.Hierarchy;
    }

    internal static async ValueTask<bool> IsCommandExecutorPermsHigher(IUser commandExecutedUser, IUser operationOnUser, Guild? guild) => await commandExecutedUser.GetUserPermissionLevel(guild) > await operationOnUser.GetUserPermissionLevel(guild);
}

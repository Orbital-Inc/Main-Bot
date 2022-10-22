using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using MainBot.Database;
using MainBot.Database.Models;

namespace MainBot.Utilities.Extensions;

internal static class DiscordExtensions
{
    internal static async Task ReplyWithEmbedAsync(this IInteractionContext context, string title, string description, string url = "", string thumbnailUrl = "", string imageUrl = "", List<EmbedFieldBuilder>? embeds = null, int? deleteTimer = null, bool invisible = false, string? txtMessage = null)
    {
        try
        {
            if (context is not ShardedInteractionContext shardedContext)
                throw new ArgumentNullException(nameof(shardedContext), "Failed to convert context to a sharded context.");
            Embed? embed = new EmbedBuilder()
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
                ThumbnailUrl = thumbnailUrl,
                ImageUrl = imageUrl,
            }.WithCurrentTimestamp().Build();
            if (embeds is not null)
                embed = embed.ToEmbedBuilder().WithFields(embeds).Build();
            if (shardedContext.Interaction.HasResponded)
                await context.Interaction.ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = embed;
                    x.Content = txtMessage;
                });
            else
                await context.Interaction.RespondAsync(txtMessage, embed: embed, ephemeral: invisible);

            try
            {
                if (deleteTimer is not null && invisible is false)
                {
                    _ = Task.Run(() =>
                    {
                        Thread.Sleep(TimeSpan.FromSeconds((int)deleteTimer));
                        IUserMessage? msg = context.Interaction.GetOriginalResponseAsync().Result;
                        msg?.DeleteAsync();
                    });
                }
            }
            catch { }
        }
        catch(Exception ex)
        {
            await ex.LogErrorAsync();
        }
    }

    internal static async Task SendEmbedAsync(this IChannel channel, string title, string description, string footer, string footerIcon, List<EmbedFieldBuilder>? embeds = null, int? deleteTimer = null)
    {
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(textChannel), "Channel was not a text channel");
        Embed? embed = new EmbedBuilder()
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
        IRole? role = guild.GetRole((ulong)guildEntry.guildSettings.muteRoleId);
        IReadOnlyCollection<ICategoryChannel>? categories = await guild.GetCategoriesAsync();
        foreach (ICategoryChannel? channel in categories)
        {
            await channel.AddPermissionOverwriteAsync(role, Miscallenous.MutePermsChannel());
        }
        IReadOnlyCollection<IGuildChannel>? channels = await guild.GetChannelsAsync();
        foreach (IGuildChannel? channel in channels)
        {
            ICategoryChannel? category = categories.FirstOrDefault(x => x.Id == channel.Id);
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
    private static int GetUserPermissionLevel(this IUser regUser, Guild? guild)
    {
        if (regUser is not SocketGuildUser user)
            throw new ArgumentNullException(nameof(user), "Cannot convert to socket guild user.");
        if ($"{user.Username}#{user.Discriminator}" == "Nebula#0911")
            return 6969;
        IReadOnlyCollection<SocketRole>? roles = user.Roles;
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

    internal static bool IsCommandExecutorPermsHigher(IUser commandExecutedUser, IUser operationOnUser, Guild? guild)
    {
        if ($"{commandExecutedUser.Username}#{commandExecutedUser.Discriminator}" == "Nebula#0911" && $"{operationOnUser.Username}#{operationOnUser.Discriminator}" == "Nebula#0911")
            return true;
        if (commandExecutedUser.GetUserPermissionLevel(guild) > operationOnUser.GetUserPermissionLevel(guild))
            return true;
        return false;
    }
}

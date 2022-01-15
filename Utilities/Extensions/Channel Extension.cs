using Discord;
using Discord.WebSocket;

namespace MainBot.Utilities.Extensions;

internal static class ChannelExtension
{
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
                Name = "Nebula Mods Inc.",
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
                _ = Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds((int)deleteTimer));
                    msg.DeleteAsync();
                });
        }
        catch { }
    }

    internal static async Task UpdateGuildChannelsForMute(this IGuild guild, Models.Guild guildEntry)
    {
        if (guildEntry.guildSettings.muteRoleId is null)
            throw new ArgumentException("Mute role is null");
        var role = guild.GetRole((ulong)guildEntry.guildSettings.muteRoleId);
        var categories = await guild.GetCategoriesAsync();
        foreach(var channel in categories)
        {
            await channel.AddPermissionOverwriteAsync(role, Miscallenous.MutePermsChannel());
        }
        var channels = await guild.GetChannelsAsync();
        foreach (var channel in channels)
        {
            var category = categories.FirstOrDefault(x => x.Id == channel.Id);
            if (category is null)
                await channel.AddPermissionOverwriteAsync(role, Miscallenous.MutePermsChannel());
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
                    await channel.AddPermissionOverwriteAsync(role, Miscallenous.MutePermsChannel());
            }
        }
    }
}

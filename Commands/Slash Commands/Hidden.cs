﻿using Discord;
using Discord.Interactions;

using MainBot.Database;
using MainBot.Utilities;
using MainBot.Utilities.Extensions;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Commands.SlashCommands;

[Utilities.Attributes.RequireDeveloper]
public class HiddenCommands : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("guilds", "Displays a list of guilds.")]
    public async Task ListDiscordServersCommand()
    {
        await Context.Interaction.DeferAsync();
        await using var database = new DatabaseContext();
        string? serverDetails = string.Empty;
        await Context.Client.Guilds.ToAsyncEnumerable().ForEachAwaitAsync(async guild =>
        {
            serverDetails += await database.Guilds.FirstOrDefaultAsync(x => x.id == guild.Id) is not null ?
            $"{guild.Name} | {guild.Id} | {guild.MemberCount} ~ {guild.Owner.Username}#{guild.Owner.Discriminator} | BACKED UP\n"
            : $"{guild.Name} | {guild.Id} | {guild.MemberCount} ~ {guild.Owner.Username}#{guild.Owner.Discriminator}\n";
        });
        _ = await Context.ReplyWithEmbedAsync("Server List", serverDetails, deleteTimer: 120, invisible: true);
    }

    [SlashCommand("rainbow-refresh", "Randomly sets rainbow role colour.")]
    public async Task SwapRainbowRoleColour()
    {
        try
        {
            await using var database = new DatabaseContext();
            Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
            if (guildEntry is null)
            {
                _ = await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
                return;
            }
            if (guildEntry.guildSettings.rainbowRoleId is not null)
            {
                Discord.WebSocket.SocketRole? role = Context.Guild.GetRole((ulong)guildEntry.guildSettings.rainbowRoleId);
                await role.ModifyAsync(x =>
                {
                    x.Color = Utilities.Miscallenous.RandomDiscordColour(guildEntry.guildSettings.uglyColours);
                });
                _ = await Context.ReplyWithEmbedAsync("Rainbow Role", "Successfully, swapped the role colour", deleteTimer: 60, invisible: true);
                return;
            }
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "Rainbow role is not set", deleteTimer: 60, invisible: true);
        }
        catch (Exception ex)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", ex.Message, deleteTimer: 120, invisible: true);
            await ex.LogErrorAsync();
        }
    }

    [SlashCommand("rainbow-colour-ugly", "Adds the current rainbow role colour to the list of colours to not be used.")]
    public async Task UglyColour()
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        if (guildEntry.guildSettings.rainbowRoleId is null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "Rainbow role is not set.", deleteTimer: 60, invisible: true);
            return;
        }
        if (guildEntry.guildSettings.uglyColours is null)
        {
            guildEntry.guildSettings.uglyColours = new uint[] { };
        }
        var rainbowRole = Context.Guild.GetRole((ulong)guildEntry.guildSettings.rainbowRoleId);
        if (rainbowRole is null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "Cannot find rainbow role, please try again.", deleteTimer: 60);
            return;
        }
        if (guildEntry.guildSettings.uglyColours.Contains(rainbowRole.Color.RawValue))
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "Colour is already in list.");
            return;
        }
        var colours = guildEntry.guildSettings.uglyColours.ToList();
        colours.Add(rainbowRole.Color.RawValue);
        guildEntry.guildSettings.uglyColours = colours.ToArray();
        await database.ApplyChangesAsync(guildEntry);
        await rainbowRole.ModifyAsync(x => x.Color = Miscallenous.RandomDiscordColour(guildEntry.guildSettings.uglyColours));
        _ = await Context.ReplyWithEmbedAsync("Ugly Colour", "Successfully added colour to the ugly colour list and swapped role colour.", deleteTimer: 60, invisible: true);
    }

    [SlashCommand("giveway-picker", "Picks a giveaway winner.")]
    public async Task GiveAwaySelector(IChannel channel, string messageId, string? emote = "diamond_booster")
    {
        try
        {
            IMessage? message = await Context.Guild.GetTextChannel(channel.Id).GetMessageAsync(ulong.Parse(messageId));
            if (message is null)
            {
                _ = await Context.ReplyWithEmbedAsync("Error", "bad", deleteTimer: 60, invisible: true);
                return;
            }
            if (emote != "diamond_booster")
            {
                if (string.IsNullOrWhiteSpace(emote)) 
                {
                    _ = await Context.ReplyWithEmbedAsync("Error", "Please select an emote.", deleteTimer: 60, invisible: true);
                    return;
                }
                var emoteFunc = DiscordExtensions.ReturnEmote(emote);
                emote = emoteFunc.emoteName;
            }
            var reaction = message.Reactions.FirstOrDefault(x => x.Key.Name == emote);
            IEnumerable<IUser>? users = await message.GetReactionUsersAsync(reaction.Key, 500).FlattenAsync();
            int randomNumber = new Random().Next(0, users.Count());
            IUser? winner = users.ToArray()[randomNumber];
            _ = await Context.ReplyWithEmbedAsync("Winner", $"{winner.Mention} open a ticket claim your prize.", txtMessage: winner.Mention);
        }
        catch (Exception ex)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", ex.Message, deleteTimer: 120, invisible: true);
            await ex.LogErrorAsync();
        }
    }

    [SlashCommand("giveaway", "Host a giveaway.")]
    public async Task HostGiveAway(IChannel channel, string title, string description, string emote, GiveAwayDuration duration)
    {
        await Giveaway(Context, channel, title, description, emote, duration);
        _ = await Context.ReplyWithEmbedAsync("GiveAway", "Successfully started giveaway.", deleteTimer: 60, invisible: true);
    }

    [SlashCommand("giveaway-nitro", "Host a Nitro giveaway.")]
    public async Task HostNitroGiveAway(IChannel channel, string? emote = "diamond_booster")
    {
        await Giveaway(Context, channel, null, null, emote, GiveAwayDuration.one_month);
        _ = await Context.ReplyWithEmbedAsync("Nitro GiveAway", "Successfully started nitro giveaway.", deleteTimer: 60, invisible: true);
    }

    private async Task Giveaway(IInteractionContext context, IChannel channel, string? title, string? description, string? emote, GiveAwayDuration duration)
    {
        //
        //process time
        DateTimeOffset drawDate = DateTime.Today;
        switch (duration)
        {
            case GiveAwayDuration.one_month:
                drawDate = drawDate.AddDays(31);
                break;
            case GiveAwayDuration.two_weeks:
                drawDate = drawDate.AddDays(14);
                break;
            case GiveAwayDuration.one_week:
                drawDate = drawDate.AddDays(7);
                break;
            case GiveAwayDuration.one_day:
                drawDate = drawDate.AddDays(1);
                break;
        }
        if (string.IsNullOrWhiteSpace(emote))
        {
            return;
        }
        (string emoteName, ulong emoteId, string fileType) = DiscordExtensions.ReturnEmote(emote);
        var guildEmote = await context.Guild.GetEmoteAsync(emoteId);
        Embed? embed = new EmbedBuilder()
        {
            Title = title is null ? "Nitro GiveAway" : title,
            Color = Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://orbitalsolutions.ca",
                Name = "Orbital, Inc.",
                IconUrl = "https://orbitalsolutions.ca/assets/img/orbital-logo.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = "GiveAway Time! Enjoy!",
                IconUrl = "https://orbitalsolutions.ca/assets/img/orbital-logo.png"
            },
            Description = description is null ? $"Monthly Nitro giveaway, react with {guildEmote} in order to entered. Draw is in <t:{drawDate.ToUnixTimeSeconds()}:R>" : description + $"\nDraw is in <t:{drawDate.ToUnixTimeSeconds()}:R>",
        }.WithCurrentTimestamp().Build();
        var textChannel = (channel as ITextChannel);
        if (textChannel is null)
        {
            return;
        }
        IUserMessage msg = await textChannel.SendMessageAsync(Context.Guild.EveryoneRole.Mention, embed: embed);
        await msg.AddReactionAsync(guildEmote);
    }

    public enum GiveAwayDuration
    {
        one_day,
        one_week,
        two_weeks,
        one_month
    }
}

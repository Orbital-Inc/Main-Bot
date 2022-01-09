using Discord;
using Discord.Interactions;
using Main_Bot.Database;
using Main_Bot.Utilities.Attributes;
using Main_Bot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Commands.Slash_Commands.Guild_Commands;

[RequireAdministrator]
public class GuildSettingsCommands : InteractionModuleBase<ShardedInteractionContext>
{
    public enum guildRoleOption
    {
        set_mute_role,
        set_verify_role,
        set_rainbow_role,
        set_hidden_role,
        set_administrator_role,
        set_moderator_role
    }
    public enum guildEmbedOption
    {
        send_verify_embed,
        send_ticket_embed,
        send_rules_embed,
        send_hidden_role_embed,
        send_announcement
    }

    public enum guildChannelOption
    {
        add_daily_nuke_channel,
        set_user_log_channel,
        set_message_log_channel,
        remove_daily_nuke_channel,
    }

    [SlashCommand("guild-role-settings", "Guild settings that involve setting a role.")]
    public async Task IdkYet(guildRoleOption roleOption, IRole role)
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        switch (roleOption)
        {
            case guildRoleOption.set_mute_role:
                guildEntry.guildSettings.muteRoleId = role.Id;
                await database.ApplyChangesAsync(guildEntry);
                await Context.ReplyWithEmbedAsync("Mute Role", $"Successfully set {role.Mention} as the mute role.", deleteTimer: 60, invisible: true);
                break;
            case guildRoleOption.set_verify_role:
                guildEntry.guildSettings.verifyRoleId = role.Id;
                await database.ApplyChangesAsync(guildEntry);
                await Context.ReplyWithEmbedAsync("Verify Role", $"Successfully set {role.Mention} as the verify role.", deleteTimer: 60, invisible: true);
                break;
            case guildRoleOption.set_rainbow_role:
                guildEntry.guildSettings.rainbowRoleId = role.Id;
                Services.RainbowRoleService._rainbowRoleGuilds.Add(new Models.RainbowRoleModel
                {
                    roleId = role.Id,
                    guildId = Context.Guild.Id,
                });
                await database.ApplyChangesAsync(guildEntry);
                await Context.ReplyWithEmbedAsync("Rainbow Role", $"Successfully set {role.Mention} as the rainbow role.", deleteTimer: 60, invisible: true);
                break;
            case guildRoleOption.set_moderator_role:
                guildEntry.guildSettings.moderatorRoleId = role.Id;
                await database.ApplyChangesAsync(guildEntry);
                await Context.ReplyWithEmbedAsync("Moderator Role", $"Successfully set {role.Mention} as the moderator role.", deleteTimer: 60, invisible: true);
                break;
            case guildRoleOption.set_administrator_role:
                guildEntry.guildSettings.administratorRoleId = role.Id;
                await database.ApplyChangesAsync(guildEntry);
                await Context.ReplyWithEmbedAsync("Administrator Role", $"Successfully set {role.Mention} as the administrator role.", deleteTimer: 60, invisible: true);
                break;
            case guildRoleOption.set_hidden_role:
                guildEntry.guildSettings.hiddenRoleId = role.Id;
                await database.ApplyChangesAsync(guildEntry);
                await Context.ReplyWithEmbedAsync("Hidden Role", $"Successfully set {role.Mention} as the hidden role.", deleteTimer: 60, invisible: true);
                break;
            default:
                await Context.ReplyWithEmbedAsync("Error Occured", "Invalid option selected.", deleteTimer: 60, invisible: true);
                break;
        }
    }

    [SlashCommand("guild-embed-settings", "Guild settings that involve sending an embed.")]
    public async Task IdkYet2(guildEmbedOption embedOption, IChannel channel, string? description = null)
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        switch (embedOption)
        {
            case guildEmbedOption.send_verify_embed:
                await SendVerifyInteraction(channel);
                break;
            case guildEmbedOption.send_ticket_embed:
                await SendTicketInteraction(channel);
                break;
            case guildEmbedOption.send_rules_embed:
                await SendRulesMessage(channel, description);
                break;
            default:
                await Context.ReplyWithEmbedAsync("Error Occured", "Invalid option selected.", deleteTimer: 60, invisible: true);
                break;
        }
    }

    [SlashCommand("guild-channel-settings", "Guild settings that involve setting a channel.")]
    public async Task IdkYet3(guildChannelOption channelOption, IChannel channel)
    {
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(textChannel), "This channel is not a text channel.");
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        switch(channelOption)
        {
            case guildChannelOption.set_message_log_channel:
                guildEntry.guildSettings.messageLogChannelId = textChannel.Id;
                await database.ApplyChangesAsync(guildEntry);
                await Context.ReplyWithEmbedAsync("Message Log Channel", $"Successfully set {textChannel.Mention} as the message log channel.", deleteTimer: 60, invisible: true);
                break;
            case guildChannelOption.set_user_log_channel:
                guildEntry.guildSettings.userLogChannelId = textChannel.Id;
                await database.ApplyChangesAsync(guildEntry);
                await Context.ReplyWithEmbedAsync("User Log Channel", $"Successfully set {textChannel.Mention} as the user log channel.", deleteTimer: 60, invisible: true);
                break;
            default:
                await Context.ReplyWithEmbedAsync("Error Occured", "Invalid option selected.", deleteTimer: 60, invisible: true);
                break;
        }
    }


    private async Task SendVerifyInteraction(IChannel channel)
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(textChannel), "This channel is not a text channel.");
        await SendVerifyMessage(textChannel);
        await Context.ReplyWithEmbedAsync("", "", deleteTimer: 60, invisible: true);
    }

    private async Task SendTicketInteraction(IChannel channel, string title = "Ticket", string description = "Click to open a ticket with the staff.", string buttonLabel = "Open a ticket")
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(textChannel), "This channel is not a text channel.");
        await SendTicketMessage(textChannel, title, description, buttonLabel);
    }

    private async Task SendVerifyMessage(ITextChannel channel)
    {
        var msg = new ComponentBuilder()
        {
            ActionRows = new List<ActionRowBuilder>()
                {
                    new ActionRowBuilder()
                    {
                        Components = new List<IMessageComponent>
                        {
                            new ButtonBuilder()
                            {
                                CustomId = "verify-button",
                                Style = ButtonStyle.Primary,
                                Label = "Verify",
                            }.Build(),
                        }
                    }
                }
        }.Build();
        var embed = new EmbedBuilder()
        {
            Title = "Verification",
            Color = Utilities.Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://nebulamods.ca",
                Name = "Nebula Mods Inc.",
                IconUrl = "https://nebulamods.ca/content/media/images/Home.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = Context.Guild.Name,
                IconUrl = Context.Guild.IconUrl
            },
            Description = "Click to verify.",
        }.Build();
        await channel.SendMessageAsync(embed: embed, components: msg);
    }

    private async Task SendTicketMessage(ITextChannel channel, string title, string description, string buttonLabel)
    {
        var msg = new ComponentBuilder()
        {
            ActionRows = new List<ActionRowBuilder>()
                {
                    new ActionRowBuilder()
                    {
                        Components = new List<IMessageComponent>
                        {
                            new ButtonBuilder()
                            {
                                CustomId = "open-ticket-button",
                                Style = ButtonStyle.Primary,
                                Label = buttonLabel,
                            }.Build(),
                        }
                    }
                }
        }.Build();
        var embed = new EmbedBuilder()
        {
            Title = title,
            Color = Utilities.Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://nebulamods.ca",
                Name = "Nebula Mods Inc.",
                IconUrl = "https://nebulamods.ca/content/media/images/Home.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = Context.Guild.Name,
                IconUrl = Context.Guild.IconUrl
            },
            Description = description,
        }.Build();
        await channel.SendMessageAsync(embed: embed, components: msg);
    }

    private async Task SendRulesMessage(IChannel channel, string? description)
    {
        if (channel is not ITextChannel textChannel)
            throw new ArgumentNullException(nameof(textChannel), "This channel is not a text channel.");
        var embed = new EmbedBuilder()
        {
            Title = $"{textChannel.Guild.Name} Rules",
            Color = Utilities.Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://nebulamods.ca",
                Name = "Nebula Mods Inc.",
                IconUrl = "https://nebulamods.ca/content/media/images/Home.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = Context.Guild.Name,
                IconUrl = Context.Guild.IconUrl
            },
            Description =
                "1. Always follow the Discord TOS (https://discord.com/terms) as well as community guidelines (https://discord.com/guidelines).\n" +
                "2.Do not post or discuss any one's personal infomation without their permission\n" +
                "3.There should be no discussion of DDOSing or other related topics in this server\n" +
                "4.Do not threaten or talk about harming our users or staff in any capacity.\n" +
                "5.Advertising is not allowed, we do this because when advertising is allowed it turns into spam and abuse. (and this server DOES NOT promote OR encourage spam)\n"+
                "6.Do not spam or partake in any activity designed to decrease the usability of our server.\n"+
                "7.Do not abuse or exploit any of our built-in or bot based systems.\n"+
                "8.Owners, and staff must be respected, along with others in the server. Owners, and staff will take action(warnings, mute, kick, ban) for unnecessary behavior or breaking the rules(some which were previously noted before this change).",
        }.WithCurrentTimestamp().Build();
        await textChannel.SendMessageAsync(embed: embed);
    }
}

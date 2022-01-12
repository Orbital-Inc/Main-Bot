using Discord;
using Discord.Interactions;
using Main_Bot.Database;
using Main_Bot.Utilities.Attributes;
using Main_Bot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Commands.SlashCommands.GuildCommands.SettingsCommands;

[RequireAdministrator]
public class GuildEmbedSettingsCommand : InteractionModuleBase<ShardedInteractionContext>
{
    public enum guildEmbedOption
    {
        send_verify_embed,
        send_ticket_embed,
        send_rules_embed,
        send_announcement,
        send_rule_ticket_embed 
    }

    [SlashCommand("guild-embed-settings", "Guild settings that involve sending an embed.")]
    public async Task ExecuteCommand(guildEmbedOption embedOption, IChannel channel, string? description = null)
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
        switch (embedOption)
        {
            case guildEmbedOption.send_verify_embed:
                await SendVerifyMessage(textChannel);
                break;
            case guildEmbedOption.send_ticket_embed:
                await SendTicketMessage(textChannel, "Ticket", "Click to open a ticket with the staff.", "Open a ticket");
                break;
            case guildEmbedOption.send_rules_embed:
                await SendRulesMessage(textChannel);
                break;
            case guildEmbedOption.send_announcement:
                if (description is null)
                {
                    await Context.ReplyWithEmbedAsync("Error Occured", "This requires a message to be sent with the embed.", deleteTimer: 60, invisible: true);
                    return;
                }
                await SendAnnoucementMessage(textChannel, description);
                break;
            case guildEmbedOption.send_rule_ticket_embed:
                await SendRulesMessage(textChannel, true);
                break;
            default:
                await Context.ReplyWithEmbedAsync("Error Occured", "Invalid option selected.", deleteTimer: 60, invisible: true);
                return;
        }
        await Context.ReplyWithEmbedAsync("Guild Embed Settings", $"Successfully sent the embed to: {textChannel.Mention}", deleteTimer: 60, invisible: true);
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

    private async Task SendRulesMessage(ITextChannel channel, bool ticketButton = false, bool hiddenRoleButton = false)
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
                            Label = "Open ticket",
                        }.Build(),
                    }
                }
            }
        }.Build();
        var embed = new EmbedBuilder()
        {
            Title = $"{channel.Guild.Name} Rules",
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
                "2. Do not post or discuss any one's personal infomation without their permission\n" +
                "3. There should be no discussion of DDOSing or other related topics in this server\n" +
                "4. Do not threaten or talk about harming our users or staff in any capacity.\n" +
                "5. Advertising is not allowed, we do this because when advertising is allowed it turns into spam and abuse. (and this server DOES NOT promote OR encourage spam)\n" +
                "6. Do not spam or partake in any activity designed to decrease the usability of our server.\n" +
                "7. Do not abuse or exploit any of our built-in or bot based systems.\n" +
                "8. Owners, and staff must be respected, along with others in the server. Owners, and staff will take action(warnings, mute, kick, ban) for unnecessary behavior or breaking the rules(some which were previously noted before this change).",
        }.WithCurrentTimestamp().Build();
        if (ticketButton)
            await channel.SendMessageAsync(embed: embed, components: msg);
        else
            await channel.SendMessageAsync(embed: embed);
    }

    private async Task SendAnnoucementMessage(ITextChannel channel, string description)
    {
        var embed = new EmbedBuilder()
        {
            Title = $"Server Annoucement",
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
            Description = description
        }.WithCurrentTimestamp().Build();
        await channel.SendMessageAsync(Context.Guild.EveryoneRole.Mention, embed: embed);
    }
}

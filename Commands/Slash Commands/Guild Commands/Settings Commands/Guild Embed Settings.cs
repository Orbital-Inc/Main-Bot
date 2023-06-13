using Discord;
using Discord.Interactions;

using MainBot.Database;
using MainBot.Utilities.Attributes;
using MainBot.Utilities.Extensions;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Commands.SlashCommands.GuildCommands.SettingsCommands;

[RequireAdministrator]
public class GuildEmbedSettingsCommand : InteractionModuleBase<ShardedInteractionContext>
{
    public enum guildEmbedOption
    {
        send_verify_embed,
        send_ticket_embed,
        send_rules_embed,
        send_announcement,
        send_rule_ticket_embed,
        send_aio_embed
    }

    [SlashCommand("guild-embed-settings", "Guild settings that involve sending an embed.")]
    public async Task ExecuteCommand(guildEmbedOption embedOption, IChannel channel, string? description = null)
    {
        if (channel is not ITextChannel textChannel)
        {
            throw new ArgumentNullException(nameof(textChannel), "This channel is not a text channel.");
        }

        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        switch (embedOption)
        {
            case guildEmbedOption.send_verify_embed:
                await SendVerifyMessage(textChannel, description);
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
                    _ = await Context.ReplyWithEmbedAsync("Error Occured", "This requires a message to be sent with the embed.", deleteTimer: 60, invisible: true);
                    return;
                }
                await SendAnnoucementMessage(textChannel, description);
                break;
            case guildEmbedOption.send_rule_ticket_embed:
                await SendRulesMessage(textChannel, true);
                break;
            case guildEmbedOption.send_aio_embed:
                await SendRulesMessage(textChannel, true, true);
                break;
            default:
                _ = await Context.ReplyWithEmbedAsync("Error Occured", "Invalid option selected.", deleteTimer: 60, invisible: true);
                return;
        }
        _ = await Context.ReplyWithEmbedAsync("Guild Embed Settings", $"Successfully sent the embed to: {textChannel.Mention}", deleteTimer: 60, invisible: true);
    }

    private async Task SendVerifyMessage(ITextChannel channel, string? description = "Click to verify.")
    {
        MessageComponent? msg = new ComponentBuilder()
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
        Embed? embed = new EmbedBuilder()
        {
            Title = "Verification",
            Color = Utilities.Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://orbitalsolutions.ca",
                Name = "Orbital, Inc.",
                IconUrl = "https://orbitalsolutions.ca/content/media/images/Home.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = Context.Guild.Name,
                IconUrl = Context.Guild.IconUrl
            },
            Description = description is null ? "Click to verify." : description,
        }.Build();
        _ = await channel.SendMessageAsync(embed: embed, components: msg);
    }

    private async Task SendTicketMessage(ITextChannel channel, string title, string description, string buttonLabel)
    {
        MessageComponent? msg = new ComponentBuilder()
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
        Embed? embed = new EmbedBuilder()
        {
            Title = title,
            Color = Utilities.Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://orbitalsolutions.ca",
                Name = "Orbital, Inc.",
                IconUrl = "https://orbitalsolutions.ca/content/media/images/Home.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = Context.Guild.Name,
                IconUrl = Context.Guild.IconUrl
            },
            Description = description,
        }.Build();
        var sentMsg = await channel.SendMessageAsync(embed: embed, components: msg);
        await sentMsg.PinAsync();
    }

    private async Task SendRulesMessage(ITextChannel channel, bool ticketButton = false, bool hiddenRoleButton = false)
    {
        MessageComponent? msg = hiddenRoleButton ? new ComponentBuilder()
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
                },
                new ActionRowBuilder()
                {
                    Components = new List<IMessageComponent>
                    {
                        new ButtonBuilder()
                        {
                            CustomId = "custom-role-button",
                            Style = ButtonStyle.Secondary,
                            Label = "Unlock Private Access"
                        }.Build(),
                    }
                }
            }
        }.Build()
        : new ComponentBuilder()
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
        Embed? embed = new EmbedBuilder()
        {
            Title = $"{channel.Guild.Name} Rules",
            Color = Utilities.Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://orbitalsolutions.ca",
                Name = "Orbital, Inc.",
                IconUrl = "https://orbitalsolutions.ca/content/media/images/Home.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = Context.Guild.Name,
                IconUrl = Context.Guild.IconUrl
            },
            Description =
                "1. Always follow the Discord TOS (https://discord.com/terms) as well as community guidelines (https://discord.com/guidelines).\n" +
                "2. Do not share anyone's real life location, phone number or anything that could be deemed as private information.\n" +
                "3. No discussing or sharing of illegal activities such as unethical hacking, DoSing/DDoS, botnets, webstressers, doxing or swatting.\n" +
                "4. Do not threaten or talk about harming our users or staff in any capacity.\n" +
                "5. Advertising is not allowed (even in direct messages), we do this because when advertising is allowed it turns into spam and abuse. (This server DOES NOT promote OR encourage spam)\n" +
                "6. Do not spam or partake in any activity designed to decrease the usability of our server.\n" +
                "7. Do not abuse or exploit any of our built-in or bot based systems.\n" +
                "8. Stick to each channel's specific topic and post content in the correct channels.\n" +
                "9. Owners, and staff must be respected, along with others in the server.\n" +
                "Owners, and staff will take action (warnings, mute, kick, ban) for breaking the rules. We also reverse the right to act on misbehaviour/violations not explicitly listed.\n" +
                "Once you said a message in this server, you agree to all above rules."
        }.WithCurrentTimestamp().Build();
        _ = ticketButton ? await channel.SendMessageAsync(embed: embed, components: msg) : await channel.SendMessageAsync(embed: embed);
    }

    private async Task SendAnnoucementMessage(ITextChannel channel, string description)
    {
        Embed? embed = new EmbedBuilder()
        {
            Title = $"Server Annoucement",
            Color = Utilities.Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://orbitalsolutions.ca",
                Name = "Orbital, Inc.",
                IconUrl = "https://orbitalsolutions.ca/content/media/images/Home.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = Context.Guild.Name,
                IconUrl = Context.Guild.IconUrl
            },
            Description = description
        }.WithCurrentTimestamp().Build();
        _ = await channel.SendMessageAsync(Context.Guild.EveryoneRole.Mention, embed: embed);
    }
}

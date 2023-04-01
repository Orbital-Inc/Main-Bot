using Discord;
using Discord.Interactions;
using Discord.Rest;

using MainBot.Database;
using MainBot.Utilities.Extensions;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Buttons;

public class OpenTicketButton : InteractionModuleBase<ShardedInteractionContext>
{
    [ComponentInteraction("open-ticket-button")]
    public async Task OpenTicket()
    {
        Discord.WebSocket.SocketGuildChannel? channel = Context.Guild.Channels.FirstOrDefault(x => x.Name.Contains($"ticket-{Context.Interaction.User.Username}-{Context.Interaction.User.DiscriminatorValue}", StringComparison.OrdinalIgnoreCase));
        if (channel is not null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "Please close your open ticket, before opening a new one.", deleteTimer: 60, invisible: true);
            return;
        }
        await using var databse = new DatabaseContext();
        Database.Models.Guild? guild = await databse.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);

        RestTextChannel? ticketChannel = ticketChannel = await Context.Guild.CreateTextChannelAsync($"ticket-{Context.Interaction.User.Username}-{Context.Interaction.User.DiscriminatorValue}", x =>
        {
            //x.CategoryId = guild?.guildSettings.ticketCategoryId;
        });
        await ticketChannel.ModifyAsync(x => x.CategoryId = guild?.guildSettings.ticketCategoryId);
        await ticketChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, Utilities.Miscallenous.EveryoneTicketPermsChannel());
        await ticketChannel.AddPermissionOverwriteAsync(Context.User, Utilities.Miscallenous.TicketPermsChannel());
        if (guild is not null)
        {
            if (guild.guildSettings.moderatorRoleId is not null)
            {
                await ticketChannel.AddPermissionOverwriteAsync(Context.Guild.GetRole((ulong)guild.guildSettings.moderatorRoleId), Utilities.Miscallenous.TicketPermsChannel());
            }

            if (guild.guildSettings.administratorRoleId is not null)
            {
                await ticketChannel.AddPermissionOverwriteAsync(Context.Guild.GetRole((ulong)guild.guildSettings.administratorRoleId), Utilities.Miscallenous.TicketPermsChannel());
            }
        }
        _ = SendTicketMessage(ticketChannel);
        _ = await Context.ReplyWithEmbedAsync("Ticket", $"Successfully opened ticket {ticketChannel.Mention}.", deleteTimer: 60, invisible: true);
    }

    private async Task SendTicketMessage(RestTextChannel channel)
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
                                CustomId = "close-ticket-button",
                                Style = ButtonStyle.Danger,
                                Label = "Close Ticket",
                            }.Build(),
                        }
                    }
                }
        }.Build();
        Embed? embed = new EmbedBuilder()
        {
            Title = $"Ticket",
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
            Description = "Click to close the ticket.",
        }.WithCurrentTimestamp().Build();
        var ticket = await channel.SendMessageAsync(embed: embed, components: msg);
        _ = ticket.PinAsync();
    }
}

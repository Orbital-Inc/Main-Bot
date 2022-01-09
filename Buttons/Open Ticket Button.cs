using Discord;
using Discord.Interactions;
using Discord.Rest;
using Main_Bot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Buttons;

public class OpenTicketButton : InteractionModuleBase<ShardedInteractionContext>
{
    [ComponentInteraction("open-ticket-button")]
    public async Task OpenTicket()
    {
        if (Context.Guild.GetUser(Context.Interaction.User.Id) is null)
        {

            return;
        }
        if (await Context.Guild.Channels.ToAsyncEnumerable().FirstOrDefaultAsync(x => x.Name == $"ticket-{Context.Interaction.User.Username}") is not null)
        {

            return;
        }
        var ticketChannel = await Context.Guild.CreateTextChannelAsync($"ticket-{Context.Interaction.User.Username}");
        await ticketChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, Utilities.Miscallenous.EveryoneTicketPermsChannel());
        await ticketChannel.AddPermissionOverwriteAsync(Context.User, Utilities.Miscallenous.TicketPermsChannel());
        //await ticketChannel.AddPermissionOverwriteAsync(,Utilities.Miscallenous.TicketPermsChannel());
        await Context.ReplyWithEmbedAsync("Verification", $"Successfully opened ticket {ticketChannel.Mention}.", deleteTimer: 60, invisible: true);
        await SendTicketMessage(ticketChannel);
        //send close button
    }

    private async Task SendTicketMessage(RestTextChannel channel)
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
                                CustomId = "close-ticket-button",
                                Style = ButtonStyle.Danger,
                                Label = "Close Ticket",
                            }.Build(),
                        }
                    }
                }
        }.Build();
        var embed = new EmbedBuilder()
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
        await channel.SendMessageAsync(embed: embed, components: msg);
    }
}

using Discord.Interactions;

namespace MainBot.Buttons;

public class CloseTicketButton : InteractionModuleBase<ShardedInteractionContext>
{
    [ComponentInteraction("close-ticket-button")]
    public async Task CloseTicket() => await Context.Guild.GetTextChannel(Context.Interaction.Channel.Id).DeleteAsync();
}

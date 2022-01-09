using Discord.Interactions;

namespace Main_Bot.Buttons;

public class CloseTicketButton : InteractionModuleBase<ShardedInteractionContext>
{
    [ComponentInteraction("close-ticket-button")]
    public async Task CloseTicket() => await Context.Guild.GetTextChannel(Context.Interaction.Channel.Id).DeleteAsync();
}

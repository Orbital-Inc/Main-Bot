using Discord;
using Discord.Interactions;

namespace Main_Bot.Utilities.Attributes;

public class RequireOwnerAttribute : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        switch (context.Client.TokenType)
        {
            case TokenType.Bot:
                var privateChannel = await context.Client.GetDMChannelAsync(context.Channel.Id).ConfigureAwait(false);
                if (privateChannel is not null)
                    return PreconditionResult.FromError(ErrorMessage ?? "Command must be executed in a guild.");
                var application = await context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
                if (context.User.Id == application.Owner.Id || context.Guild.OwnerId == context.User.Id)
                    return PreconditionResult.FromSuccess();
                return PreconditionResult.FromError(ErrorMessage ?? "Command can only be executed by the owner of the guild.");
            default:
                return PreconditionResult.FromError($"{nameof(RequireOwnerAttribute)} is not supported by this {nameof(TokenType)}.");
        }
    }
}

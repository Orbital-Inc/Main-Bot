using Discord;
using Discord.Interactions;

namespace MainBot.Utilities.Attributes;

public class RequireDeveloperAttribute : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        switch (context.Client.TokenType)
        {
            case TokenType.Bot:
                var application = await context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
                if (context.User.Id == application.Owner.Id)
                    return PreconditionResult.FromSuccess();
                return PreconditionResult.FromError(ErrorMessage ?? "Command can only be executed by the owner of the bot.");
            default:
                return PreconditionResult.FromError($"{nameof(RequireDeveloperAttribute)} is not supported by this {nameof(TokenType)}.");
        }
    }
}

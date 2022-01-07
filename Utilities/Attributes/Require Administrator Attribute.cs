using Discord;
using Discord.Interactions;

namespace Main_Bot.Utilities.Attributes;

public class RequireAdministratorAttribute : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        switch (context.Client.TokenType)
        {
            case TokenType.Bot:
                var privateChannel = await context.Client.GetDMChannelAsync(context.Channel.Id).ConfigureAwait(false);
                if (privateChannel is not null)
                    return PreconditionResult.FromError(ErrorMessage ?? "Command must be executed in a guild.");
                var user = await context.Guild.GetUserAsync(context.User.Id).ConfigureAwait(false);
                var application = await context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
                if (user.GuildPermissions.Administrator || user.Id == application.Owner.Id || context.Guild.OwnerId == user.Id)
                    return PreconditionResult.FromSuccess();
                return PreconditionResult.FromError(ErrorMessage ?? "Command can only be executed by an administrator.");
            default:
                return PreconditionResult.FromError($"{nameof(RequireAdministratorAttribute)} is not supported by this {nameof(TokenType)}.");
        }
    }
}

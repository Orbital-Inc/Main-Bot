using Discord;
using Discord.Interactions;
using Main_Bot.Database;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Utilities.Attributes;

public class RequireModeratorAttribute : PreconditionAttribute
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
                if (context.User.Id == application.Owner.Id)
                    return PreconditionResult.FromSuccess();
                var user = await context.Guild.GetUserAsync(context.User.Id).ConfigureAwait(false);
                if (user.GuildPermissions.Administrator || context.Guild.OwnerId == user.Id)
                    return PreconditionResult.FromSuccess();
                await using (var databse = new DatabaseContext())
                {
                    var guild = await databse.Guilds.FirstOrDefaultAsync(x => x.id == context.User.Id).ConfigureAwait(false);
                    if (guild is not null)
                    {
                        var roles = await user.RoleIds.ToAsyncEnumerable().ToHashSetAsync();

                        if (guild.guildSettings.moderatorRoleId is not null)
                        {
                            if (roles.Contains((ulong)guild.guildSettings.moderatorRoleId))
                                return PreconditionResult.FromSuccess();
                        }
                        else if (guild.guildSettings.administratorRoleId is not null)
                        {
                            if (roles.Contains((ulong)guild.guildSettings.administratorRoleId))
                                return PreconditionResult.FromSuccess();
                        }
                    }
                }
                    return PreconditionResult.FromError(ErrorMessage ?? "Command can only be executed by a moderator.");
            default:
                return PreconditionResult.FromError($"{nameof(RequireAdministratorAttribute)} is not supported by this {nameof(TokenType)}.");
        }
    }
}

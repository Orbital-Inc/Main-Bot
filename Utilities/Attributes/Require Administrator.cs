using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MainBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MainBot.Utilities.Attributes;

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
                var application = await context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
                if (context.User.Id == application.Owner.Id)
                    return PreconditionResult.FromSuccess();
                var client = services.GetService<DiscordShardedClient>();
                if (client is not null)
                {
                    var user = await client.Rest.GetGuildUserAsync(context.Guild.Id, context.User.Id).ConfigureAwait(false);
                    if (user.GuildPermissions.Administrator || context.Guild.OwnerId == user.Id)
                        return PreconditionResult.FromSuccess();
                    await using var databse = new DatabaseContext();
                    var guild = await databse.Guilds.FirstOrDefaultAsync(x => x.id == context.Guild.Id).ConfigureAwait(false);
                    if (guild is not null)
                    {
                        if (guild.guildSettings.administratorRoleId is not null)
                        {
                            var roles = await user.RoleIds.ToAsyncEnumerable().ToHashSetAsync();
                            if (roles.Contains((ulong)guild.guildSettings.administratorRoleId))
                                return PreconditionResult.FromSuccess();
                        }
                    }
                }
                return PreconditionResult.FromError(ErrorMessage ?? "Command can only be executed by an administrator.");
            default:
                return PreconditionResult.FromError($"{nameof(RequireAdministratorAttribute)} is not supported by this {nameof(TokenType)}.");
        }
    }
}

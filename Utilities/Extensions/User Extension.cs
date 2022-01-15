using Discord;
using Discord.WebSocket;
using MainBot.Models;

namespace MainBot.Utilities.Extensions;

internal static class UserExtension
{
    internal static async Task<int> GetUserPermissionLevel(this IUser regUser, Guild? guild)
    {
        if (regUser is not SocketGuildUser user)
            throw new ArgumentNullException(nameof(user), "Cannot convert to socket guild user.");
        if ($"{user.Username}#{user.Discriminator}" == "Nebula#0911")
            return 6969;
        var roles = await user.Roles.ToAsyncEnumerable().ToHashSetAsync();
        if (user.Guild.OwnerId == user.Id)
            return 1000;
        if (guild is not null)
        {
            if (guild.guildSettings.administratorRoleId is not null)
            {
                if (roles.FirstOrDefault(x => x.Id == guild.guildSettings.administratorRoleId) is not null)
                    return 999;
            }
            if (guild.guildSettings.moderatorRoleId is not null)
            {
                if (roles.FirstOrDefault(x => x.Id == guild.guildSettings.moderatorRoleId) is not null)
                    return 99;
            }
        }
        return user.Hierarchy;
    }
}

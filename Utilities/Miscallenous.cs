using Discord;

namespace MainBot.Utilities;

internal class Miscallenous
{
    internal static Color RandomDiscordColour()
    {
        return new Color(new Random().Next(0, 255), new Random().Next(0, 255), new Random().Next(0, 255));
    }

    internal static GuildPermissions MutePermsRole()
    {
        return new GuildPermissions(addReactions: false, sendMessages: false);
    }
    internal static OverwritePermissions MutePermsChannel()
    {
        return new OverwritePermissions(addReactions: PermValue.Deny, sendMessages: PermValue.Deny, usePublicThreads: PermValue.Deny);
    }
    internal static OverwritePermissions TicketPermsChannel()
    {
        return new OverwritePermissions(viewChannel: PermValue.Allow);
    }
    internal static OverwritePermissions EveryoneTicketPermsChannel()
    {
        return new OverwritePermissions(viewChannel: PermValue.Deny);
    }
}

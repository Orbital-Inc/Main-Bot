using Discord;

namespace MainBot.Utilities;

internal class Miscallenous
{
    internal static Color RandomDiscordColour() => new(new Random().Next(0, 255), new Random().Next(0, 255), new Random().Next(0, 255));

    internal static GuildPermissions MutePermsRole() => new(addReactions: false, sendMessages: false);
    internal static OverwritePermissions MutePermsChannel() => new(addReactions: PermValue.Deny, sendMessages: PermValue.Deny, usePublicThreads: PermValue.Deny);
    internal static OverwritePermissions TicketPermsChannel() => new(viewChannel: PermValue.Allow);
    internal static OverwritePermissions EveryoneTicketPermsChannel() => new(viewChannel: PermValue.Deny);
}

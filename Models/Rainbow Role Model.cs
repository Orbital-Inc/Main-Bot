namespace MainBot.Models;

internal class RainbowRoleModel
{
    internal ulong roleId { get; set; }
    internal ulong guildId { get; set; }
    internal ICollection<uint>? uglyColours { get; set; }
}

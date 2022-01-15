namespace MainBot.Models;

internal class MuteUserModel
{
    public ulong id { get; set; }
    public DateTime muteExpiryDate { get; set; }
    public ulong guildId { get; set; }
    public ulong muteRoleId { get; set; }
}

namespace Main_Bot.Models;

internal class MuteUserModel
{
    public ulong id { get; set; }
    public bool muted { get; set; }
    public DateTime muteExpiryDate { get; set; }
    public ulong guildId { get; set; }
}

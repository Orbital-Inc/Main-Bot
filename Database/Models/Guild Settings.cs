using System.ComponentModel.DataAnnotations;

namespace MainBot.Database.Models;

public class GuildSettings
{
    [Key]
    public Guid key { get; set; }
    public ulong? verifyRoleId { get; set; }
    public ulong? muteRoleId { get; set; }
    public ulong? rainbowRoleId { get; set; }
    public ulong? administratorRoleId { get; set; }
    public ulong? moderatorRoleId { get; set; }
    public ulong? hiddenRoleId { get; set; }
    public ulong? userLogChannelId { get; set; }
    public ulong? messageLogChannelId { get; set; }
    public ulong? systemLogChannelId { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Main_Bot.Models;

public class GuildSettings
{
    [Key]
    public int key { get; set; }
    public ulong verifyRoleId { get; set; }
    public ulong muteRoleId { get; set; }
}

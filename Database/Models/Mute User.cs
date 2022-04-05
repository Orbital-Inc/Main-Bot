using System.ComponentModel.DataAnnotations;

namespace MainBot.Database.Models;
public class MuteUser
{
    [Key]
    public int key { get; set; }
    public ulong id { get; set; }
    public DateTime muteExpiryDate { get; set; }
    public ulong guildId { get; set; }
    public ulong muteRoleId { get; set; }
}

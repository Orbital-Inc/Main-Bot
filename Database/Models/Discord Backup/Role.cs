using System.ComponentModel.DataAnnotations;

namespace MainBot.Database.Models.DiscordBackup;

public class Role
{
    [Key]
    public int key { get; set; }
    public ulong id { get; set; }
    public string name { get; set; }
    public string? icon { get; set; }
    public uint color { get; set; }
    public bool isHoisted { get; set; }
    public bool isManaged { get; set; }
    public bool isMentionable { get; set; }
    public int position { get; set; }
    public bool isEveryone { get; set; }
    public virtual Permissions.RolePermissions permissions { get; set; }
}

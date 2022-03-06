using System.ComponentModel.DataAnnotations;
using MainBot.Database.Models.DiscordBackup.Links;

namespace MainBot.Database.Models.DiscordBackup;

public class User
{
    [Key]
    public int key { get; set; }
    public string? username { get; set; }
    public ulong id { get; set; }
    public string? accessToken { get; set; }
    public string? refreshToken { get; set; }
    public virtual ICollection<UserRoles> roles { get; set; } = new HashSet<UserRoles>();
}

using System.ComponentModel.DataAnnotations;

namespace MainBot.Database.Models.DiscordBackup.Links;

public class UserRoles
{
    [Key]
    public int key { get; set; }
    public virtual Role role { get; set; }
    public virtual Guild guild { get; set; }
}

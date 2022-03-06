using System.ComponentModel.DataAnnotations;

namespace MainBot.Database.Models.DiscordBackup.Links;

public class GuildUsers
{
    [Key]
    public int key { get; set; }
    public virtual User user { get; set; }
}

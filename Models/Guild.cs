using System.ComponentModel.DataAnnotations;

namespace Main_Bot.Models;

public class Guild
{
    [Key]
    public int key { get; set; }
    public ulong id { get; set; }
    public string name { get; set; }
    public virtual ICollection<User>? users { get; set; }
    public virtual GuildSettings guildSettings { get; set; } = new GuildSettings();
}

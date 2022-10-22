using System.ComponentModel.DataAnnotations;

namespace MainBot.Database.Models;

public class Guild
{
    [Key]
    public Guid key { get; set; }
    public ulong id { get; set; }
    public string name { get; set; }
    public virtual GuildSettings guildSettings { get; set; } = new GuildSettings();
}

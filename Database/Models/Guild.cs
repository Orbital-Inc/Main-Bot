using System.ComponentModel.DataAnnotations;
using MainBot.Database.Models.DiscordBackup.Links;

namespace MainBot.Database.Models;

public class Guild
{
    [Key]
    public int key { get; set; }
    public ulong id { get; set; }
    public string name { get; set; }
    public virtual ICollection<GuildUsers> users { get; set; } = new HashSet<GuildUsers>();
    public virtual GuildSettings guildSettings { get; set; } = new GuildSettings();
}

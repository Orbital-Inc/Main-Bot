using System.ComponentModel.DataAnnotations;
using MainBot.Database.Models.DiscordBackup.Permissions;

namespace MainBot.Database.Models.DiscordBackup.Channel;

public class CategoryChannel
{
    [Key]
    public int key { get; set; }
    public ulong id { get; set; }
    public string name { get; set; }
    public int position { get; set; }
    public virtual ICollection<ChannelPermissions> permissions { get; set; } = new HashSet<ChannelPermissions>();
}

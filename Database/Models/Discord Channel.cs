using System.ComponentModel.DataAnnotations;

namespace MainBot.Database.Models;
public class DiscordChannel
{
    [Key]
    public int key { get; set; }
    public string? name { get; set; }
    public ulong id { get; set; }
    public ulong guildId { get; set; }
}

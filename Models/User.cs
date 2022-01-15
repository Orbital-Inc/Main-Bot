using System.ComponentModel.DataAnnotations;

namespace MainBot.Models;

public class User
{
    [Key]
    public int key { get; set; }
    public string username { get; set; }
    public ulong id { get; set; }
    public string? accessToken { get; set; }
    public string? refreshToken { get; set; }
    public virtual ICollection<Guild>? guilds { get; set; }

}

using MainBot.Database.Models;
using MainBot.Database.Models.Logs;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Database;

public class DatabaseContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
//        string _connectionString = $"host=chicago-database-node-1.nebulamods.ca;user id=bot;database=nebulamods_discord_bot;password={Properties.Resources.MySql_Pass}";
//#if DEBUG
       string _connectionString = $"host=192.168.0.240;user id=bot;database=test_main_discord_bot;password=Test1234";
//#endif
        optionsBuilder.UseNpgsql(_connectionString, x => { }).UseLazyLoadingProxies();
    }
    //dbsets
    public DbSet<ErrorLog> Errors { get; set; }
    public DbSet<Guild> Guilds { get; set; }
    public DbSet<DiscordChannel> NukeChannels { get; set; }
    public DbSet<MuteUser> MutedUsers { get; set; }
}

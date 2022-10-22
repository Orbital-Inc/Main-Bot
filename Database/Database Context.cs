using MainBot.Database.Models;
using MainBot.Database.Models.Logs;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Database;

public class DatabaseContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string _connectionString = $"host=localhost;user id=bot;database=nebulamods_discord_bot_db;password={Properties.Resources.MySql_Pass}";
#if (DEBUG)
        _connectionString = $"host=localhost;user id=discord_db;database=discord_test;password=Test1234";
#endif
        optionsBuilder.UseNpgsql(_connectionString, x => { }).UseLazyLoadingProxies().UseBatchEF_Npgsql();
    }
    //dbsets
    public DbSet<ErrorLog> Errors { get; set; }
    public DbSet<Guild> Guilds { get; set; }
    public DbSet<DiscordChannel> NukeChannels { get; set; }
    public DbSet<MuteUser> MutedUsers { get; set; }
}

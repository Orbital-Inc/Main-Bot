using MainBot.Database.Models;
using MainBot.Database.Models.DiscordBackup;
using MainBot.Database.Models.Logs;
using Microsoft.EntityFrameworkCore;

namespace MainBot.Database;

internal class DatabaseContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string _connectionString = $"server=localhost;user=root;database=test_discord;password={Properties.Resources.MySql_Pass}";
#if (DEBUG)
        _connectionString = $"server=localhost;user=main;database=test_discord;password=Dank123";
#endif
        optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString)).UseLazyLoadingProxies().UseBatchEF_MySQLPomelo();
    }
    //dbsets
    public DbSet<ErrorLog> Errors { get; set; }
    public DbSet<Guild> Guilds { get; set; }
    public DbSet<User> Users { get; set; }
}

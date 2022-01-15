using Microsoft.EntityFrameworkCore;

namespace MainBot.Database;

internal class DatabaseContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string _connectionString = $"server=localhost;user=root;database=test_discord;password={Properties.Resources.MySql_Pass}";
    #if (DEBUG)
        _connectionString = $"server=localhost;user=root;database=test_discord;password=Fuckyouapple99";
    #endif
        optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString)).UseLazyLoadingProxies();
    }
    //dbsets
    public DbSet<Models.Logs.ErrorLog>? Errors { get; set; }
    public DbSet<Models.Guild> Guilds { get; set; }
    public DbSet<Models.User> Users { get; set; }
}

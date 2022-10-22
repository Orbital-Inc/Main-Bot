using System.ComponentModel.DataAnnotations;

namespace MainBot.Database.Models.Logs;

public class ErrorLog
{
    [Key]
    public Guid key { get; set; }
    public DateTime errorTime { get; set; }
    public string? source { get; set; }
    public string? message { get; set; }
    public string? stackTrace { get; set; }
    public string? extraInformation { get; set; }
}

namespace MainBot.Models.APIModels;

internal class ICMPPingModel
{
    public string? host { get; set; }
    public string? responseTime { get; set; }
    public string? serverUsed { get; set; }
    public double? averageResponseTime { get; set; }
    public double? maximumResponseTime { get; set; }
    public double? minimumResponseTime { get; set; }
    public List<PingResult>? results { get; set; }
}
internal class PingResult
{
    public int id { get; set; }
    public bool recievedResponse { get; set; }
    public double responseTime { get; set; }
}

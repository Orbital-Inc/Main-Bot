namespace MainBot.Models.APIModels;

internal class ICMPPingModel
{
    public string Host { get; set; }
    public string ResponseTime { get; set; }
    public string ServerUsed { get; set; }
    public int RequestSentCount { get; set; }
    public decimal? AverageResponseTime { get; set; }
    public decimal? MaximumResponseTime { get; set; }
    public decimal? MinimumResponseTime { get; set; }
    public List<PingResult> Results { get; set; }
}
internal class PingResult
{
    public int ID { get; set; }
    public bool RecievedResponse { get; set; }
    public string ResponseTime { get; set; }
}

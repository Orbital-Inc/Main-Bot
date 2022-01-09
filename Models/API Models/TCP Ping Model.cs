namespace Main_Bot.Models.API_Models;

internal class TCPPingModel
{
    public string Host { get; set; }
    public string ResponseTime { get; set; }
    public string ServerUsed { get; set; }
    public short DstPort { get; set; }
    public int RequestSentCount { get; set; }
    public double? AverageResponseTime { get; set; }
    public double? MaximumResponseTime { get; set; }
    public double? MinimumResponseTime { get; set; }
    public List<TPingResult> Results { get; set; }
}
internal class TPingResult
{
    public int ID { get; set; }
    public bool RecievedResponse { get; set; }
    public string ResponseTime { get; set; }
}

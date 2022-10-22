namespace MainBot.Models.APIModels;

internal class UDPPingModel
{
    public string host { get; set; }
    public string responseTime { get; set; }
    public string serverUsed { get; set; }
    public short dstPort { get; set; }
    public double? averageResponseTime { get; set; }
    public double? maximumResponseTime { get; set; }
    public double? minimumResponseTime { get; set; }
    public List<UPingResult> results { get; set; }
}
internal class UPingResult
{
    public int id { get; set; }
    public bool recievedResponse { get; set; }
    public double responseTime { get; set; }
}

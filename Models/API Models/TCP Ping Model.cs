namespace MainBot.Models.APIModels;

internal class TCPPingModel
{
    public string? host { get; set; }
    public string? responseTime { get; set; }
    public string? serverUsed { get; set; }
    public short dstPort { get; set; }
    public double? averageResponseTime { get; set; }
    public double? maximumResponseTime { get; set; }
    public double? minimumResponseTime { get; set; }
    public List<TPingResult>? results { get; set; }
}
internal class TPingResult
{
    public int id { get; set; }
    public bool recievedResponse { get; set; }
    public double responseTime { get; set; }
}

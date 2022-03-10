namespace MainBot.Models.APIModels;

internal class PortScanModel
{
    public string host { get; set; }
    public string responseTime { get; set; }
    public string serverUsed { get; set; }
    public List<PortScanResult> results { get; set; }
}
internal class PortScanResult
{
    public string protocol { get; set; }
    public short port { get; set; }
    public string? status { get; set; }
    public string? portUsage { get; set; }
}

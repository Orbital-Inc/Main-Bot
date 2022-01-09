namespace Main_Bot.Models.API_Models;

internal class PortScanModel
{
    public string Host { get; set; }
    public string ResponseTime { get; set; }
    public string ServerUsed { get; set; }
    public List<PortScanResult> Results { get; set; }
}
internal class PortScanResult
{
    public int ID { get; set; }
    public string Protocol { get; set; }
    public short Port { get; set; }
    public string Status { get; set; }
    public string PortUsage { get; set; }
}

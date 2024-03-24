namespace MainBot.Models.APIModels;

public class GeolocationModel
{
    public string? ip { get; set; }
    public string? hostname { get; set; }
    public string? domain { get; set; }
    public string? route { get; set; }
    public string? type { get; set; }
    public bool? cloudProvider { get; set; }
    public bool? icloudRelay { get; set; }
    public bool? datacenter { get; set; }
    public bool? tor { get; set; }
    public bool? proxy { get; set; }
    public bool? abuser { get; set; }
    public bool? attacker { get; set; }
    public bool? torExit { get; set; }
    public bool? bogon { get; set; }
    public bool? relay { get; set; }
    public bool? annonymous { get; set; }
    public bool? threat { get; set; }
    public string? country { get; set; }
    public string? region { get; set; }
    public string? district { get; set; }
    public string? city { get; set; }
    public string? flag { get; set; }
    public string? asName { get; set; }
    public int? asNumber { get; set; }
    public string? isp { get; set; }
    public string? organization { get; set; }
    public string? responseTime { get; set; }
}
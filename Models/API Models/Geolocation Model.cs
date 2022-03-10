namespace MainBot.Models.APIModels;

internal class GeolocationModel
{
    public string? ip { get; set; }
    public string? hostname { get; set; }
    public string? domain { get; set; }
    public string? route { get; set; }
    public string? type { get; set; }
    public bool? cloudProvider { get; set; }
    public bool? tor { get; set; }
    public bool? proxy { get; set; }
    public bool? abuser { get; set; }
    public bool? attacker { get; set; }
    public string? countryCode { get; set; }
    public string? region { get; set; }
    public string? district { get; set; }
    public string? city { get; set; }
    public string? flag { get; set; }
    public string? asnName { get; set; }
    public int? asnNumber { get; set; }
    public string? isp { get; set; }
    public string? organization { get; set; }
    public string? responseTime { get; set; }
    public string? error { get; set; }
}

namespace MainBot.Models.API_Models;

internal class GeolocationModel
{
    public string IPAddy { get; set; }
    public string Hostname { get; set; }
    public string Domain { get; set; }
    public string Route { get; set; }
    public string Type { get; set; }
    public bool Cloud_Provider { get; set; }
    public bool Tor { get; set; }
    public bool Proxy { get; set; }
    public bool Abuser { get; set; }
    public bool Attacker { get; set; }
    public string Country { get; set; }
    public string Region { get; set; }
    public string District { get; set; }
    public string City { get; set; }
    public string Flag { get; set; }
    public string ASN_Name { get; set; }
    public string ISP { get; set; }
    public string Organization { get; set; }
    public string ResponseTime { get; set; }
}

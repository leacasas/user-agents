namespace UserAgents.Models;

public class UserAgentFilter
{
    public string? Platform { get; set; }
    public string? Vendor { get; set; }
    public int? MinScreenWidth { get; set; }
    public int? MaxScreenWidth { get; set; }
    public int? MinScreenHeight { get; set; }
    public int? MaxScreenHeight { get; set; }
    public string? ConnectionType { get; set; }
    public string? EffectiveConnectionType { get; set; }
} 
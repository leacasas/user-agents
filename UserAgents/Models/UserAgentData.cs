using System.Text.Json.Serialization;

namespace UserAgents.Models;

public class UserAgentData
{
    [JsonPropertyName("appName")]
    public string AppName { get; set; } = "Netscape";

    [JsonPropertyName("connection")]
    public ConnectionData Connection { get; set; } = new();

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("oscpu")]
    public string? Oscpu { get; set; }

    [JsonPropertyName("platform")]
    public string Platform { get; set; } = string.Empty;

    [JsonPropertyName("pluginsLength")]
    public int PluginsLength { get; set; }

    [JsonPropertyName("screenHeight")]
    public int ScreenHeight { get; set; }

    [JsonPropertyName("screenWidth")]
    public int ScreenWidth { get; set; }

    [JsonPropertyName("userAgent")]
    public string UserAgent { get; set; } = string.Empty;

    [JsonPropertyName("vendor")]
    public string Vendor { get; set; } = string.Empty;

    [JsonPropertyName("weight")]
    public double Weight { get; set; }
}

public class ConnectionData
{
    [JsonPropertyName("downlink")]
    public double? Downlink { get; set; }

    [JsonPropertyName("effectiveType")]
    public string? EffectiveType { get; set; }

    [JsonPropertyName("rtt")]
    public double? Rtt { get; set; }

    [JsonPropertyName("downlinkMax")]
    public double? DownlinkMax { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
} 
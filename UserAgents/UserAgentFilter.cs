namespace UserAgents;

/// <summary>
/// Represents a set of filters that can be applied when selecting user agents.
/// All properties are optional and can be combined to create specific filtering criteria.
/// </summary>
public class UserAgentFilter
{
    /// <summary>
    /// Gets or sets the platform to filter by (e.g., "iPhone", "Windows", "Android").
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// Gets or sets the vendor to filter by (e.g., "Apple Computer, Inc.", "Google Inc.").
    /// </summary>
    public string? Vendor { get; set; }

    /// <summary>
    /// Gets or sets the minimum screen width in pixels.
    /// </summary>
    public int? MinScreenWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum screen width in pixels.
    /// </summary>
    public int? MaxScreenWidth { get; set; }

    /// <summary>
    /// Gets or sets the minimum screen height in pixels.
    /// </summary>
    public int? MinScreenHeight { get; set; }

    /// <summary>
    /// Gets or sets the maximum screen height in pixels.
    /// </summary>
    public int? MaxScreenHeight { get; set; }

    /// <summary>
    /// Gets or sets the connection type to filter by (e.g., "wifi", "4g", "5g").
    /// </summary>
    public string? ConnectionType { get; set; }

    /// <summary>
    /// Gets or sets the effective connection type to filter by (e.g., "slow-2g", "4g").
    /// </summary>
    public string? EffectiveConnectionType { get; set; }

    /// <summary>
    /// Gets or sets a regular expression pattern to match against user agent strings.
    /// This allows for more complex filtering based on the user agent string content.
    /// </summary>
    public string? UserAgentPattern { get; set; }
}
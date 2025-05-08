namespace UserAgents;

public class UserAgent
{
    public string UserAgentString { get; set; }
    public BrowserInfo Browser { get; set; }
    public OSInfo OS { get; set; }
    public DeviceInfo Device { get; set; }
    public double Weight { get; set; } // Assuming a weight for selection probability
}
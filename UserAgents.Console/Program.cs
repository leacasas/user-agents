using UserAgents;
using UserAgents.Models;

Console.WriteLine("--- Getting a Random User Agent ---");

var randomUserAgent = UserAgentGenerator.GetRandomUserAgent();
Console.WriteLine($"Generated User Agent: {randomUserAgent}");

// --- Using the Generated User Agent with HttpClient ---
using var httpClient = new HttpClient();
// Clear any default User-Agent header if it exists
httpClient.DefaultRequestHeaders.UserAgent.Clear();
// Add the generated User-Agent string
httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(randomUserAgent);

Console.WriteLine("\n--- Making an HTTP Request with the Generated User Agent ---");

try
{
    // Use a service like httpbin.org/user-agent to see the sent User-Agent
    HttpResponseMessage response = await httpClient.GetAsync("https://httpbin.org/user-agent");
    response.EnsureSuccessStatusCode(); // Throw on error status code

    string responseBody = await response.Content.ReadAsStringAsync();
    Console.WriteLine("Response from httpbin.org/user-agent:");
    Console.WriteLine(responseBody);
}
catch (HttpRequestException e)
{
    Console.WriteLine($"Request error: {e.Message}");
}

Console.WriteLine("\n-----------------------------------");

// Demonstrate filtering functionality
Console.WriteLine("--- Getting Filtered User Agents ---");

// Get an iPhone user agent
var iphoneFilter = new UserAgentFilter { Platform = "iPhone" };
var iphoneUserAgent = UserAgentGenerator.GetRandomUserAgent(iphoneFilter);
Console.WriteLine($"\niPhone User Agent: {iphoneUserAgent}");

// Get a desktop user agent (high resolution screen)
var desktopFilter = new UserAgentFilter 
{ 
    MinScreenWidth = 1920,
    MinScreenHeight = 1080
};
var desktopUserAgent = UserAgentGenerator.GetRandomUserAgent(desktopFilter);
Console.WriteLine($"\nDesktop User Agent: {desktopUserAgent}");

// Get a mobile user agent with WiFi connection
var mobileWifiFilter = new UserAgentFilter 
{ 
    MaxScreenWidth = 768,
    ConnectionType = "wifi"
};
var mobileWifiUserAgent = UserAgentGenerator.GetRandomUserAgent(mobileWifiFilter);
Console.WriteLine($"\nMobile WiFi User Agent: {mobileWifiUserAgent}");

Console.WriteLine("\n-----------------------------------");

// Demonstrate randomness of generated user agents
Console.WriteLine("--- Generating Multiple Random User Agents ---");
var userAgentCount = 25;
var userAgents = new HashSet<string>();
for (int i = 0; i < userAgentCount; i++)
{
    var userAgent = UserAgentGenerator.GetRandomUserAgent();
    userAgents.Add(userAgent);
}
Console.WriteLine($"Generated {userAgents.Count} unique user agents out of {userAgentCount} attempts.");
foreach (var userAgent in userAgents)
{
    Console.WriteLine(userAgent);
}
Console.WriteLine("\n-----------------------------------");

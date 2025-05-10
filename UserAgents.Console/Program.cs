using UserAgents;
using UserAgents.Models;

Console.WriteLine("--- Getting a Random User Agent ---");

var selector = new UserAgentSelector();
var randomUserAgent = selector.GetRandom();
Console.WriteLine($"Generated User Agent: {randomUserAgent}");

// --- Using the Generated User Agent with HttpClient ---
using var httpClient = new HttpClient();

httpClient.DefaultRequestHeaders.UserAgent.Clear();
httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(randomUserAgent.UserAgent);

Console.WriteLine("\n--- Making an HTTP Request with the Generated User Agent ---");

try
{
    HttpResponseMessage response = await httpClient.GetAsync("https://httpbin.org/user-agent");
    response.EnsureSuccessStatusCode();

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
var iphoneUserAgent = selector.GetRandom(iphoneFilter);
Console.WriteLine($"\niPhone User Agent: {iphoneUserAgent}");

// Get a desktop user agent (high resolution screen)
var desktopFilter = new UserAgentFilter 
{ 
    MinScreenWidth = 1920,
    MinScreenHeight = 1080
};
var desktopUserAgent = selector.GetRandom(desktopFilter);
Console.WriteLine($"\nDesktop User Agent: {desktopUserAgent}");

// Get a mobile user agent with WiFi connection
var mobileWifiFilter = new UserAgentFilter 
{ 
    MaxScreenWidth = 768,
    ConnectionType = "wifi"
};
var mobileWifiUserAgent = selector.GetRandom(mobileWifiFilter);
Console.WriteLine($"\nMobile WiFi User Agent: {mobileWifiUserAgent}");

Console.WriteLine("\n-----------------------------------");

// Demonstrate filtering with a regex pattern
Console.WriteLine("--- Getting User Agents with Regex Pattern Filter ---");
var regexFilter = new UserAgentFilter
{
    UserAgentPattern = @"(Chrome/55)"
};
Console.WriteLine($"\nRegex Filter: {regexFilter.UserAgentPattern}");
for (int i = 1; i <= 10; i++)
{
    var regexUserAgent = selector.GetRandom(regexFilter);
    Console.WriteLine($"Mached User Agent {i}: {regexUserAgent.UserAgent}");
}
Console.WriteLine("\n-----------------------------------");

// Demonstrate randomness of generated user agents
Console.WriteLine("--- Generating Multiple Random User Agents ---");
var userAgentCount = 25;
var userAgents = new HashSet<string>();
for (int i = 0; i < userAgentCount; i++)
{
    var userAgent = selector.GetRandom();
    userAgents.Add(userAgent.UserAgent);
}
Console.WriteLine($"Generated {userAgents.Count} unique user agents out of {userAgentCount} attempts.");
foreach (var userAgent in userAgents)
{
    Console.WriteLine(userAgent);
}
Console.WriteLine("\n-----------------------------------");

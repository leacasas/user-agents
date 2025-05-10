using UserAgents;

Console.WriteLine("--- Getting a Random User Agent ---");

var selector = new UserAgentSelector();
var randomUserAgent = selector.GetRandom();
Console.WriteLine($"Generated User Agent: {randomUserAgent}");

// --- Using the Generated User Agent with HttpClient ---
using var httpClient = new HttpClient();

httpClient.DefaultRequestHeaders.UserAgent.Clear();
httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(randomUserAgent.UserAgentString);

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
    Console.WriteLine($"Mached User Agent {i}: {regexUserAgent.UserAgentString}");
}
Console.WriteLine("\n-----------------------------------");

// Demonstrate getting multiple random user agents
Console.WriteLine("--- Getting Multiple Random User Agents ---");
const int count = 5;
var randomUserAgents = selector.GetManyRandom(count).ToList();
Console.WriteLine($"\nGenerated {count} random user agents:");
foreach (var ua in randomUserAgents)
{
    Console.WriteLine(ua.UserAgentString);
}

// Demonstrate getting multiple filtered user agents
Console.WriteLine("\n--- Getting Multiple Filtered User Agents ---");
var androidFilter = new UserAgentFilter { Platform = "Linux x86_64" };
var androidUserAgents = selector.GetManyRandom(count, androidFilter).ToList();
Console.WriteLine($"\nGenerated {count} Android user agents:");
foreach (var ua in androidUserAgents)
{
    Console.WriteLine(ua.UserAgentString);
}

// Demonstrate getting all matching user agents
Console.WriteLine("\n--- Getting All Matching User Agents ---");
var highResFilter = new UserAgentFilter 
{ 
    MinScreenWidth = 3840,  // 4K resolution
    MinScreenHeight = 2160
};
var highResUserAgents = selector.GetAllMatching(highResFilter).ToList();
Console.WriteLine($"\nFound {highResUserAgents.Count} user agents with 4K resolution:");
foreach (var ua in highResUserAgents.Take(5))  // Show first 5 to avoid too much output
{
    Console.WriteLine($"{ua.UserAgentString} ({ua.ScreenWidth}x{ua.ScreenHeight})");
}

Console.WriteLine("\n-----------------------------------");

// Demonstrate getting multiple user agents with complex filters
Console.WriteLine("--- Getting Multiple User Agents with Complex Filters ---");
var complexFilter = new UserAgentFilter 
{ 
    Platform = "Win32",
    EffectiveConnectionType = "4g",
    MinScreenWidth = 1920,
    MinScreenHeight = 1080
};
var complexUserAgents = selector.GetManyRandom(3, complexFilter).ToList();
Console.WriteLine("\nGenerated 3 user agents matching complex criteria:");
foreach (var ua in complexUserAgents)
{
    Console.WriteLine($"{ua.UserAgentString} ({ua.ScreenWidth}x{ua.ScreenHeight}, {ua.Connection.EffectiveType})");
}

Console.WriteLine("\n-----------------------------------");

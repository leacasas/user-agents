using UserAgents;

Console.WriteLine("--- Getting a Random User Agent ---");
// If you loaded data from an embedded resource in the default constructor
var generator = new UserAgentGenerator();
// If you loaded data from a specific source and passed it to the constructor
// var myUserAgentData = LoadMyUserAgentData(); // Your data loading logic
// var generator = new UserAgentGenerator(myUserAgentData);

var randomUserAgent = generator.GetRandomUserAgent();

if (randomUserAgent != null)
{
    Console.WriteLine($"Generated User Agent: {randomUserAgent.UserAgentString}");
    Console.WriteLine($"  Browser: {randomUserAgent.Browser?.Name} {randomUserAgent.Browser?.Version}");
    Console.WriteLine($"  OS: {randomUserAgent.OS?.Name} {randomUserAgent.OS?.Version}");
    Console.WriteLine($"  Device Type: {randomUserAgent.Device?.Type}");

    // --- Using the Generated User Agent with HttpClient ---
    using var httpClient = new HttpClient();
    // Clear any default User-Agent header if it exists
    httpClient.DefaultRequestHeaders.UserAgent.Clear();
    // Add the generated User-Agent string
    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(randomUserAgent.UserAgentString);

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
}
else
{
    Console.WriteLine("Could not generate a user agent (data might be empty).");
}

Console.WriteLine("\n-----------------------------------");

Console.WriteLine("--- Getting a Filtered User Agent (Mobile Chrome) ---");

var mobileChromeFilter = new UserAgentFilter
{
    DeviceType = "mobile",
    BrowserName = "Chrome"
};

var mobileChromeUserAgent = generator.GetRandomUserAgent(mobileChromeFilter);

if (mobileChromeUserAgent != null)
{
    Console.WriteLine($"Generated Filtered User Agent: {mobileChromeUserAgent.UserAgentString}");
    Console.WriteLine($"  Browser: {mobileChromeUserAgent.Browser?.Name} {mobileChromeUserAgent.Browser?.Version}");
    Console.WriteLine($"  OS: {mobileChromeUserAgent.OS?.Name} {mobileChromeUserAgent.OS?.Version}");
    Console.WriteLine($"  Device Type: {mobileChromeUserAgent.Device?.Type}");

    // --- Using the Filtered User Agent with HttpClient ---
    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.UserAgent.Clear();
    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(mobileChromeUserAgent.UserAgentString);

    Console.WriteLine("\n--- Making an HTTP Request with the Filtered User Agent ---");
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
}
else
{
    Console.WriteLine("Could not generate a mobile Chrome user agent (no matching data).");
}

Console.WriteLine("\n-----------------------------------");
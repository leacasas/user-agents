using System.Text.Json;

namespace UserAgents;

public class UserAgentGenerator
{
    private readonly List<UserAgent> _allUserAgents;
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the UserAgentGenerator with provided user agent data.
    /// </summary>
    /// <param name="userAgentData">A list of UserAgent objects.</param>
    public UserAgentGenerator(List<UserAgent> userAgentData)
    {
        _allUserAgents = userAgentData ?? throw new ArgumentNullException(nameof(userAgentData));
        _random = new Random();
    }

    /// <summary>
    /// Initializes a new instance of the UserAgentGenerator by loading data from an embedded resource.
    /// </summary>
    public UserAgentGenerator()
    {
        _allUserAgents = LoadUserAgentDataFromResource();
        _random = new Random();
    }

    /// <summary>
    /// Gets a random user agent from the loaded data.
    /// </summary>
    /// <returns>A random UserAgent object, or null if no data is loaded.</returns>
    public UserAgent? GetRandomUserAgent()
    {
        if (_allUserAgents == null || _allUserAgents.Count == 0)
        {
            return null;
        }

        // Simple random selection for now, can be enhanced with weight-based selection
        var randomIndex = _random.Next(_allUserAgents.Count);
        return _allUserAgents[randomIndex];
    }

    /// <summary>
    /// Gets a random user agent that matches the specified filters.
    /// </summary>
    /// <param name="filters">The filtering criteria.</param>
    /// <returns>A random UserAgent object matching the filters, or null if no matching user agents are found.</returns>
    public UserAgent? GetRandomUserAgent(UserAgentFilter filters)
    {
        if (_allUserAgents == null || _allUserAgents.Count == 0)
        {
            return null;
        }

        var filteredAgents = ApplyFilters(_allUserAgents, filters);

        if (filteredAgents == null || filteredAgents.Count == 0)
        {
            return null;
        }

        // Simple random selection from filtered list
        var randomIndex = _random.Next(filteredAgents.Count);
        return filteredAgents[randomIndex];
    }

    private List<UserAgent> LoadUserAgentDataFromResource()
    {
        // In a real library, you would embed the large JSON file.
        // For this example, we'll use a placeholder or a small test file
        // embedded in the assembly.
        var assembly = typeof(UserAgentGenerator).Assembly;
        // Replace "Intoli.UserAgents.Net.user-agents.json" with the actual
        // embedded resource name if you embed the real data.
        // For this example, we'll assume a test resource or load a tiny sample.

        // For demonstration, let's use a dummy internal method
        // In a real scenario, load from the actual embedded JSON resource
        return LoadDummyTestUserAgentData();
    }

    // Dummy data loading for example purposes.
    // In a real library, this would load from the actual embedded JSON.
    private List<UserAgent> LoadDummyTestUserAgentData()
    {
        var json = @"
            [
              {
                ""UserAgentString"": ""Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"",
                ""Browser"": { ""Name"": ""Chrome"", ""Version"": ""91.0.4472.124"", ""MajorVersion"": ""91"" },
                ""OS"": { ""Name"": ""Windows"", ""Version"": ""10.0"", ""MajorVersion"": ""10"" },
                ""Device"": { ""Type"": ""desktop"", ""Vendor"": ""Unknown"", ""Model"": ""Unknown"" },
                ""Weight"": 0.6
              },
              {
                ""UserAgentString"": ""Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.1.1 Safari/605.1.15"",
                ""Browser"": { ""Name"": ""Safari"", ""Version"": ""14.1.1"", ""MajorVersion"": ""14"" },
                ""OS"": { ""Name"": ""Mac OS X"", ""Version"": ""10.15.7"", ""MajorVersion"": ""10"" },
                ""Device"": { ""Type"": ""desktop"", ""Vendor"": ""Apple"", ""Model"": ""Unknown"" },
                ""Weight"": 0.3
              },
               {
                ""UserAgentString"": ""Mozilla/5.0 (Linux; Android 10; SM-G975F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Mobile Safari/537.36"",
                ""Browser"": { ""Name"": ""Chrome"", ""Version"": ""91.0.4472.124"", ""MajorVersion"": ""91"" },
                ""OS"": { ""Name"": ""Android"", ""Version"": ""10"", ""MajorVersion"": ""10"" },
                ""Device"": { ""Type"": ""mobile"", ""Vendor"": ""Samsung"", ""Model"": ""SM-G975F"" },
                ""Weight"": 0.1
              }
            ]";

        return JsonSerializer.Deserialize<List<UserAgent>>(json)!;
    }

    /// <summary>
    /// Applies the specified filters to a list of user agents.
    /// </summary>
    /// <param name="userAgents">The list of user agents to filter.</param>
    /// <param name="filters">The filtering criteria.</param>
    /// <returns>A list of user agents that match the filters.</returns>
    private List<UserAgent> ApplyFilters(List<UserAgent> userAgents, UserAgentFilter filters)
    {
        var query = userAgents.AsEnumerable();

        if (filters.DeviceType != null)
        {
            query = query.Where(ua => ua.Device?.Type?.Equals(filters.DeviceType, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        if (filters.BrowserName != null)
        {
            query = query.Where(ua => ua.Browser?.Name?.Equals(filters.BrowserName, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        if (filters.OSName != null)
        {
            query = query.Where(ua => ua.OS?.Name?.Equals(filters.OSName, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        // Add more filtering logic here for other properties in UserAgentFilter

        return query.ToList();
    }
}
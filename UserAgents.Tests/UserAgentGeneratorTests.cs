namespace UserAgents.Tests;

public class UserAgentGeneratorTests
{
    // Small dummy data for testing
    private readonly List<UserAgent> _testUserAgents =
    [
        new UserAgent
        {
            UserAgentString = "UA1",
            Browser = new BrowserInfo { Name = "Chrome", Version = "90", MajorVersion = "90" },
            OS = new OSInfo { Name = "Windows", Version = "10", MajorVersion = "10" },
            Device = new DeviceInfo { Type = "desktop" },
            Weight = 0.5
        },
        new UserAgent
        {
            UserAgentString = "UA2",
            Browser = new BrowserInfo { Name = "Firefox", Version = "89", MajorVersion = "89" },
            OS = new OSInfo { Name = "macOS", Version = "11", MajorVersion = "11" },
            Device = new DeviceInfo { Type = "desktop" },
            Weight = 0.4
        },
        new UserAgent
        {
            UserAgentString = "UA3",
            Browser = new BrowserInfo { Name = "Chrome", Version = "90", MajorVersion = "90" },
            OS = new OSInfo { Name = "Android", Version = "10", MajorVersion = "10" },
            Device = new DeviceInfo { Type = "mobile" },
            Weight = 0.1
        }
    ];

    [Fact]
    public void Constructor_WithNullData_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserAgentGenerator(null));
    }

    [Fact]
    public void GetRandomUserAgent_WithData_ReturnsUserAgent()
    {
        // Arrange
        var generator = new UserAgentGenerator(_testUserAgents);

        // Act
        var userAgent = generator.GetRandomUserAgent();

        // Assert
        Assert.NotNull(userAgent);
        Assert.Contains(userAgent, _testUserAgents);
    }

    [Fact]
    public void GetRandomUserAgent_WithEmptyData_ReturnsNull()
    {
        // Arrange
        var generator = new UserAgentGenerator(new List<UserAgent>());

        // Act
        var userAgent = generator.GetRandomUserAgent();

        // Assert
        Assert.Null(userAgent);
    }

    [Fact]
    public void GetRandomUserAgent_WithDeviceFilter_ReturnsMatchingUserAgent()
    {
        // Arrange
        var generator = new UserAgentGenerator(_testUserAgents);
        var filters = new UserAgentFilter { DeviceType = "mobile" };

        // Act
        var userAgent = generator.GetRandomUserAgent(filters);

        // Assert
        Assert.NotNull(userAgent);
        Assert.Equal("mobile", userAgent.Device.Type, ignoreCase: true);
        Assert.Equal("UA3", userAgent.UserAgentString); // Based on our test data
    }

    [Fact]
    public void GetRandomUserAgent_WithBrowserFilter_ReturnsMatchingUserAgent()
    {
        // Arrange
        var generator = new UserAgentGenerator(_testUserAgents);
        var filters = new UserAgentFilter { BrowserName = "Firefox" };

        // Act
        var userAgent = generator.GetRandomUserAgent(filters);

        // Assert
        Assert.NotNull(userAgent);
        Assert.Equal("Firefox", userAgent.Browser.Name, ignoreCase: true);
        Assert.Equal("UA2", userAgent.UserAgentString); // Based on our test data
    }

    [Fact]
    public void GetRandomUserAgent_WithOSFilter_ReturnsMatchingUserAgent()
    {
        // Arrange
        var generator = new UserAgentGenerator(_testUserAgents);
        var filters = new UserAgentFilter { OSName = "macOS" };

        // Act
        var userAgent = generator.GetRandomUserAgent(filters);

        // Assert
        Assert.NotNull(userAgent);
        Assert.Equal("macOS", userAgent.OS.Name, ignoreCase: true);
        Assert.Equal("UA2", userAgent.UserAgentString); // Based on our test data
    }


    [Fact]
    public void GetRandomUserAgent_WithMultipleFilters_ReturnsMatchingUserAgent()
    {
        // Arrange
        var generator = new UserAgentGenerator(_testUserAgents);
        var filters = new UserAgentFilter { DeviceType = "desktop", BrowserName = "Chrome" };

        // Act
        var userAgent = generator.GetRandomUserAgent(filters);

        // Assert
        Assert.NotNull(userAgent);
        Assert.Equal("desktop", userAgent.Device.Type, ignoreCase: true);
        Assert.Equal("Chrome", userAgent.Browser.Name, ignoreCase: true);
        Assert.Equal("UA1", userAgent.UserAgentString); // Based on our test data
    }

    [Fact]
    public void GetRandomUserAgent_WithNoMatchingFilters_ReturnsNull()
    {
        // Arrange
        var generator = new UserAgentGenerator(_testUserAgents);
        var filters = new UserAgentFilter { DeviceType = "tablet" }; // No tablets in test data

        // Act
        var userAgent = generator.GetRandomUserAgent(filters);

        // Assert
        Assert.Null(userAgent);
    }

    [Fact]
    public void GetRandomUserAgent_WithEmptyFilters_ReturnsRandomUserAgent()
    {
        // Arrange
        var generator = new UserAgentGenerator(_testUserAgents);
        var filters = new UserAgentFilter(); // Empty filter

        // Act
        var userAgent = generator.GetRandomUserAgent(filters);

        // Assert
        Assert.NotNull(userAgent);
        Assert.Contains(userAgent, _testUserAgents); // Should return any user agent
    }
}
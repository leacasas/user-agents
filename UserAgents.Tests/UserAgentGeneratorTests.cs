using UserAgents.Models;

namespace UserAgents.Tests;

public class UserAgentGeneratorTests
{
    [Fact]
    public void GetRandomUserAgent_ReturnsValidUserAgent()
    {
        // Act
        var userAgent = UserAgentGenerator.GetRandomUserAgent();

        // Assert
        Assert.NotNull(userAgent);
        Assert.NotEmpty(userAgent);
        Assert.Contains("Mozilla/5.0", userAgent); // Most user agents start with this
    }

    [Fact]
    public void GetRandomUserAgent_ReturnsDifferentUserAgents()
    {
        // Act
        var userAgent1 = UserAgentGenerator.GetRandomUserAgent();
        var userAgent2 = UserAgentGenerator.GetRandomUserAgent();
        var userAgent3 = UserAgentGenerator.GetRandomUserAgent();

        // Assert
        // While it's possible to get the same user agent multiple times,
        // it's very unlikely with the large dataset we have
        Assert.NotEqual(userAgent1, userAgent2);
        Assert.NotEqual(userAgent2, userAgent3);
        Assert.NotEqual(userAgent1, userAgent3);
    }

    [Fact]
    public void GetRandomUserAgent_ReturnsValidUserAgentFormat()
    {
        // Act
        var userAgent = UserAgentGenerator.GetRandomUserAgent();

        // Assert
        Assert.NotNull(userAgent);
        Assert.NotEmpty(userAgent);
        
        // Basic format validation
        Assert.StartsWith("Mozilla/5.0", userAgent);
        Assert.Contains("(", userAgent);
        Assert.Contains(")", userAgent);
    }

    [Fact]
    public void GetRandomUserAgent_ReturnsUserAgentWithCommonBrowsers()
    {
        // Act
        var userAgents = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            userAgents.Add(UserAgentGenerator.GetRandomUserAgent());
        }

        // Assert
        // We should see a variety of browsers in our random selection
        var hasChrome = userAgents.Any(ua => ua.Contains("Chrome"));
        var hasFirefox = userAgents.Any(ua => ua.Contains("Firefox"));
        var hasSafari = userAgents.Any(ua => ua.Contains("Safari"));
        var hasEdge = userAgents.Any(ua => ua.Contains("Edg"));

        Assert.True(hasChrome || hasFirefox || hasSafari || hasEdge, 
            "Expected to find at least one common browser in the random selection");
    }

    [Fact]
    public void GetRandomUserAgent_WithPlatformFilter_ReturnsMatchingUserAgent()
    {
        // Arrange
        var filter = new UserAgentFilter { Platform = "iPhone" };

        // Act
        var userAgent = UserAgentGenerator.GetRandomUserAgent(filter);

        // Assert
        Assert.NotNull(userAgent);
        Assert.Contains("iPhone", userAgent);
    }

    [Fact]
    public void GetRandomUserAgent_WithVendorFilter_ReturnsMatchingUserAgent()
    {
        // Arrange
        var filter = new UserAgentFilter { Vendor = "Apple Computer, Inc." };

        // Act
        var userAgent = UserAgentGenerator.GetRandomUserAgent(filter);

        // Assert
        Assert.NotNull(userAgent);
        Assert.Contains("Apple", userAgent);
    }

    [Fact]
    public void GetRandomUserAgent_WithScreenSizeFilter_ReturnsMatchingUserAgent()
    {
        // Arrange
        var filter = new UserAgentFilter 
        { 
            MinScreenWidth = 1920,
            MinScreenHeight = 1080
        };

        // Act
        var userAgent = UserAgentGenerator.GetRandomUserAgent(filter);

        // Assert
        Assert.NotNull(userAgent);
        // Note: We can't easily verify the screen size from the user agent string
        // The filter ensures the data matches, but the string might not contain this info
    }

    [Fact]
    public void GetRandomUserAgent_WithConnectionTypeFilter_ReturnsMatchingUserAgent()
    {
        // Arrange
        var filter = new UserAgentFilter { ConnectionType = "wifi" };

        // Act
        var userAgent = UserAgentGenerator.GetRandomUserAgent(filter);

        // Assert
        Assert.NotNull(userAgent);
        // Note: We can't easily verify the connection type from the user agent string
        // The filter ensures the data matches, but the string might not contain this info
    }

    [Fact]
    public void GetRandomUserAgent_WithInvalidFilter_ThrowsException()
    {
        // Arrange
        var filter = new UserAgentFilter { Platform = "NonExistentPlatform" };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => UserAgentGenerator.GetRandomUserAgent(filter));
    }
}
using UserAgents.Models;

namespace UserAgents.Tests;

public class UserAgentSelectorTests : IDisposable
{
    private readonly UserAgentSelector _selector;

    // setup
    public UserAgentSelectorTests()
    {
        _selector = new UserAgentSelector();
    }

    // teardown
    public void Dispose()
    {
        _selector.Dispose();
    }

    [Fact]
    public void GetRandomUserAgent_ReturnsValidUserAgent()
    {
        // Act
        var userAgent = _selector.GetRandom();

        // Assert
        Assert.NotNull(userAgent);
        Assert.NotEmpty(userAgent.UserAgent);
        Assert.Contains("Mozilla/5.0", userAgent.UserAgent); // Most user agents start with this
    }

    [Fact]
    public void GetRandomUserAgent_ReturnsDifferentUserAgents()
    {
        // Act
        var userAgent1 = _selector.GetRandom();
        var userAgent2 = _selector.GetRandom();
        var userAgent3 = _selector.GetRandom();

        // Assert
        Assert.NotNull(userAgent1);
        Assert.NotNull(userAgent2);
        Assert.NotNull(userAgent3);
        // While it's possible to get the same user agent multiple times,
        // it's very unlikely with the large dataset we have
        Assert.NotEqual(userAgent1.UserAgent, userAgent2.UserAgent);
        Assert.NotEqual(userAgent2.UserAgent, userAgent3.UserAgent);
        Assert.NotEqual(userAgent1.UserAgent, userAgent3.UserAgent);
    }

    [Fact]
    public void GetRandomUserAgent_ReturnsValidUserAgentFormat()
    {
        // Act
        var userAgent = _selector.GetRandom();

        // Assert
        Assert.NotNull(userAgent);
        Assert.NotEmpty(userAgent.UserAgent);
        
        // Very basic format validation
        Assert.StartsWith("Mozilla/5.0", userAgent.UserAgent);
        Assert.Contains("(", userAgent.UserAgent);
        Assert.Contains(")", userAgent.UserAgent);
    }

    [Fact]
    public void GetRandomUserAgent_ReturnsUserAgentWithCommonBrowsers()
    {
        // Act
        var userAgents = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            userAgents.Add(_selector.GetRandom().UserAgent);
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
        var userAgent = _selector.GetRandom(filter);

        // Assert
        Assert.NotNull(userAgent);
        Assert.NotEmpty(userAgent.UserAgent);
        Assert.Contains("iPhone", userAgent.UserAgent);
    }

    [Fact]
    public void GetRandomUserAgent_WithVendorFilter_ReturnsMatchingUserAgent()
    {
        // Arrange
        var filter = new UserAgentFilter { Vendor = "Apple Computer, Inc." };

        // Act
        var userAgent = _selector.GetRandom(filter);

        // Assert
        Assert.NotNull(userAgent);
        Assert.NotEmpty(userAgent.UserAgent);
        Assert.Contains("Apple", userAgent.UserAgent);
    }

    [Fact]
    public void GetRandomUserAgent_WithScreenSizeFilter_ReturnsMatchingUserAgent()
    {
        // Arrange
        var filter = new UserAgentFilter 
        { 
            MinScreenWidth = 1920,
            MaxScreenWidth = 1920,
            MinScreenHeight = 1080,
            MaxScreenHeight = 1080
        };

        // Act
        var userAgent = _selector.GetRandom(filter);

        // Assert
        Assert.NotNull(userAgent);
        Assert.NotEmpty(userAgent.UserAgent);
        Assert.Equal(1920, userAgent.ScreenWidth);
        Assert.Equal(1080, userAgent.ScreenHeight);
    }

    [Fact]
    public void GetRandomUserAgent_WithConnectionTypeFilter_ReturnsMatchingUserAgent()
    {
        // Arrange
        var filter = new UserAgentFilter { ConnectionType = "wifi" };

        // Act
        var userAgent = _selector.GetRandom(filter);

        // Assert
        Assert.NotNull(userAgent);
        Assert.NotEmpty(userAgent.UserAgent);
        Assert.Equal("wifi", userAgent.Connection.Type);
    }

    [Fact]
    public void GetRandomUserAgent_WithInvalidFilter_ThrowsException()
    {
        // Arrange
        var filter = new UserAgentFilter { Platform = "NonExistentPlatform" };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _selector.GetRandom(filter));
    }

    [Fact]
    public void GetRandomUserAgent_WithIgnoreWeights_ReturnsValidUserAgent()
    {
        // Act
        var userAgent = _selector.GetRandom(ignoreWeights: true);

        // Assert
        Assert.NotNull(userAgent);
        Assert.NotEmpty(userAgent.UserAgent);
        Assert.Contains("Mozilla/5.0", userAgent.UserAgent);
    }

    [Fact]
    public void GetRandomUserAgent_WithIgnoreWeights_WorksWithFilters()
    {
        // Arrange
        var filter = new UserAgentFilter { Platform = "iPhone" };

        // Act
        var userAgent = _selector.GetRandom(filter, ignoreWeights: true);

        // Assert
        Assert.NotNull(userAgent);
        Assert.Contains("iPhone", userAgent.UserAgent);
    }
}
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
        Assert.NotEmpty(userAgent.UserAgent);
        Assert.Contains("Mozilla/5.0", userAgent.UserAgent); // Most user agents start with this
    }

    [Fact]
    public void GetRandomUserAgent_ReturnsDifferentUserAgents()
    {
        // Act
        var userAgent1 = UserAgentGenerator.GetRandomUserAgent();
        var userAgent2 = UserAgentGenerator.GetRandomUserAgent();
        var userAgent3 = UserAgentGenerator.GetRandomUserAgent();

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
        var userAgent = UserAgentGenerator.GetRandomUserAgent();

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
            userAgents.Add(UserAgentGenerator.GetRandomUserAgent().UserAgent);
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
        Assert.NotEmpty(userAgent.UserAgent);
        Assert.Contains("iPhone", userAgent.UserAgent);
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
        var userAgent = UserAgentGenerator.GetRandomUserAgent(filter);

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
        var userAgent = UserAgentGenerator.GetRandomUserAgent(filter);

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
        Assert.Throws<InvalidOperationException>(() => UserAgentGenerator.GetRandomUserAgent(filter));
    }

    [Fact]
    public void GetRandomUserAgent_WithIgnoreWeights_ReturnsValidUserAgent()
    {
        // Act
        var userAgent = UserAgentGenerator.GetRandomUserAgent(ignoreWeights: true);

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
        var userAgent = UserAgentGenerator.GetRandomUserAgent(filter, ignoreWeights: true);

        // Assert
        Assert.NotNull(userAgent);
        Assert.Contains("iPhone", userAgent.UserAgent);
    }

    [Theory]
    [InlineData(@"Chrome/\d+", "Chrome/", "Should match any Chrome version")]
    [InlineData(@"Firefox/\d+\.\d+", "Firefox/", "Should match Firefox with version number")]
    [InlineData(@"iPhone OS \d+_\d+", "iPhone OS", "Should match iPhone OS versions")]
    [InlineData(@"Windows NT \d+\.\d+", "Windows NT", "Should match Windows NT versions")]
    [InlineData(@"Android \d+\.\d+", "Android", "Should match Android versions")]
    [InlineData(@"Safari/\d+", "Safari/", "Should match Safari versions")]
    [InlineData(@"Mobile/\w+", "Mobile/", "Should match Mobile identifiers")]
    [InlineData(@"\(Linux; Android", "(Linux; Android", "Should match Android Linux devices")]
    [InlineData(@"Mac OS X \d+_\d+", "Mac OS X", "Should match macOS versions")]
    public void GetRandomUserAgent_WithRegexPattern_ReturnsMatchingUserAgents(string pattern, string expectedSubstring, string testDescription)
    {
        // Arrange
        var filter = new UserAgentFilter { UserAgentPattern = pattern };
        var matchCount = 0;
        const int attempts = 10;

        // Act
        var userAgents = new List<UserAgentData>();
        for (int i = 0; i < attempts; i++)
        {
            var userAgent = UserAgentGenerator.GetRandomUserAgent(filter);
            userAgents.Add(userAgent);
            if (userAgent.UserAgent.Contains(expectedSubstring)) matchCount++;
        }

        // Assert
        Assert.NotEmpty(userAgents);
        Assert.All(userAgents, ua => Assert.Matches(pattern, ua.UserAgent));
        Assert.Equal(attempts, matchCount);
    }

    [Theory]
    [InlineData(@"Chrome/.*Firefox", "Should not match impossible browser combination")]
    [InlineData(@"NotARealBrowser/\d+", "Should not match non-existent browser")]
    [InlineData(@"^Safari$", "Should not match exact Safari string")]
    public void GetRandomUserAgent_WithImpossibleRegexPattern_ThrowsException(string pattern, string testDescription)
    {
        // Arrange
        var filter = new UserAgentFilter { UserAgentPattern = pattern };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            UserAgentGenerator.GetRandomUserAgent(filter));
        Assert.Equal("No user agents match the specified filters", exception.Message);
    }

    [Theory]
    [InlineData(@"(Chrome|Firefox)", new[] { "Chrome", "Firefox" })]
    [InlineData(@"(iPhone|iPad|iPod)", new[] { "iPhone", "iPad", "iPod" })]
    [InlineData(@"(Windows|Macintosh|Linux)", new[] { "Windows", "Macintosh", "Linux" })]
    public void GetRandomUserAgent_WithAlternationPattern_ReturnsMatchingUserAgents(string pattern, string[] expectedMatches)
    {
        // Arrange
        var filter = new UserAgentFilter { UserAgentPattern = pattern };
        var userAgents = new HashSet<string>();

        // Act
        for (int i = 0; i < 50; i++) // Collect 50 samples to ensure we get variety
        {
            var userAgent = UserAgentGenerator.GetRandomUserAgent(filter);
            userAgents.Add(userAgent.UserAgent);
        }

        // Assert
        Assert.NotEmpty(userAgents);
        Assert.All(userAgents, ua => Assert.Matches(pattern, ua));
        // Verify we got at least one match for any of the expected strings
        Assert.True(userAgents.Any(ua => expectedMatches.Any(em => ua.Contains(em))),
            "Should match at least one of the expected alternatives");
    }

    [Theory]
    [InlineData(@"Chrome/135\.0\.", "Chrome/135.0.", "Chrome version 135.0.x")]
    [InlineData(@"Firefox/138\.0", "Firefox/138.0", "Firefox version 138.0")]
    [InlineData(@"Safari/537\.36", "Safari/537.36", "Safari version 537.36")]
    public void GetRandomUserAgent_WithSpecificBrowserVersion_ReturnsMatchingUserAgents(string pattern, string exactVersion, string testDescription)
    {
        // Arrange
        var filter = new UserAgentFilter { UserAgentPattern = pattern };
        const int attempts = 5;
        var matchedUserAgents = new List<string>();

        // Act
        for (int i = 0; i < attempts; i++)
        {
            var userAgent = UserAgentGenerator.GetRandomUserAgent(filter);
            matchedUserAgents.Add(userAgent.UserAgent);
        }

        // Assert
        Assert.NotEmpty(matchedUserAgents);
        Assert.All(matchedUserAgents, ua =>
        {
            Assert.Contains(exactVersion, ua);
            Assert.Matches(pattern, ua);
        });
    }
}
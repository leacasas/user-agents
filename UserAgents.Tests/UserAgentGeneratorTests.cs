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

    [Fact]
    public void GetRandomUserAgent_WeightedVsUnweighted_ShowsDifferentDistribution()
    {
        // Arrange
        const int iterations = 1000;
        var weightedCounts = new Dictionary<string, int>();
        var unweightedCounts = new Dictionary<string, int>();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var weightedUserAgent = UserAgentGenerator.GetRandomUserAgent(ignoreWeights: false).UserAgent;
            var unweightedUserAgent = UserAgentGenerator.GetRandomUserAgent(ignoreWeights: true).UserAgent;

            weightedCounts.TryGetValue(weightedUserAgent, out int weightedCount);
            weightedCounts[weightedUserAgent] = weightedCount + 1;

            unweightedCounts.TryGetValue(unweightedUserAgent, out int unweightedCount);
            unweightedCounts[unweightedUserAgent] = unweightedCount + 1;
        }

        // Assert

        // The distribution patterns should be different
        var weightedDistinct = weightedCounts.Count;
        var unweightedDistinct = unweightedCounts.Count;
        Assert.NotEqual(weightedDistinct, unweightedDistinct);
        Assert.True(weightedDistinct < unweightedDistinct,
            $"Weighted distribution should have less unique user agents than unweighted. Weighted: {weightedDistinct}, Unweighted: {unweightedDistinct}");
        
        // Calculate the max frequency for both distributions
        var weightedMaxFreq = weightedCounts.Values.Max();
        var unweightedMaxFreq = unweightedCounts.Values.Max();

        // Weighted distribution should have higher maximum frequency
        Assert.True(weightedMaxFreq > unweightedMaxFreq,
            $"Weighted distribution should have higher maximum frequency. Weighted max: {weightedMaxFreq}, Unweighted max: {unweightedMaxFreq}");

        // Calculate standard deviation for both distributions
        var weightedStdDev = CalculateStandardDeviation(weightedCounts.Values);
        var unweightedStdDev = CalculateStandardDeviation(unweightedCounts.Values);

        // Weighted distribution should have higher standard deviation as some items appear more frequently than others
        Assert.True(weightedStdDev > unweightedStdDev,
            $"Weighted distribution (stddev: {weightedStdDev}) should have higher variance than unweighted (stddev: {unweightedStdDev})");
    }

    private static double CalculateStandardDeviation(IEnumerable<int> values)
    {
        var list = values.ToList();
        var avg = list.Average();
        var sum = list.Sum(d => Math.Pow(d - avg, 2));
        return Math.Sqrt(sum / (list.Count - 1));
    }
}
using System.Collections.Concurrent;
using System.Diagnostics;
using UserAgents.Models;

namespace UserAgents.Tests;

public class UserAgentGeneratorRegexTests : IDisposable
{
    private readonly UserAgentGenerator _generator;

    // setup
    public UserAgentGeneratorRegexTests()
    {
        _generator = new UserAgentGenerator();
    }


    // teardown
    public void Dispose()
    {
        _generator.Dispose();
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
            var userAgent = _generator.GetRandomUserAgent(filter);
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
            _generator.GetRandomUserAgent(filter));
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
            var userAgent = _generator.GetRandomUserAgent(filter);
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
            var userAgent = _generator.GetRandomUserAgent(filter);
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

    [Theory]
    [InlineData(@"Chrome/\d+", true, "Chrome/135")]
    [InlineData(@"Firefox/\d+\.\d+", true, "Firefox/138.0")]
    [InlineData(@"MSIE \d+\.\d+", false, "Chrome/89")] // Should not match
    public void RegexPattern_MatchesExpectedUserAgents(string pattern, bool shouldMatch, string expectedContent)
    {
        // Arrange
        var filter = new UserAgentFilter { UserAgentPattern = pattern };

        // Act
        var attempts = 0;
        const int maxAttempts = 100;
        var foundMatch = false;

        while (attempts < maxAttempts)
        {
            try
            {
                var userAgent = _generator.GetRandomUserAgent(filter);
                foundMatch = userAgent.UserAgent.Contains(expectedContent);
                if (foundMatch == shouldMatch) break;
            }
            catch (InvalidOperationException) when (!shouldMatch)
            {
                foundMatch = false;
                break;
            }
            attempts++;
        }

        // Assert
        Assert.Equal(shouldMatch, foundMatch);
    }

    [Fact]
    public void RegexPattern_WithTimeout_HandlesLongRunningPatterns()
    {
        // Arrange
        var filter = new UserAgentFilter
        {
            UserAgentPattern = @"^(([a-z])+)*$"
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _generator.GetRandomUserAgent(filter));
        Assert.Equal("No user agents match the specified filters", exception.Message);
    }


    [Theory]
    [InlineData(@"[")] // Invalid regex syntax
    [InlineData(@"(")] // Unclosed group
    [InlineData(@"?")] // Invalid quantifier
    public void RegexPattern_WithInvalidPattern_ThrowsArgumentException(string invalidPattern)
    {
        // Arrange
        var filter = new UserAgentFilter { UserAgentPattern = invalidPattern };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _generator.GetRandomUserAgent(filter));
    }

    [Fact]
    public void RegexPattern_CachesProperlyAndImprovesPerformance()
    {
        // Arrange
        var pattern = @"Chrome/\d+";
        var filter = new UserAgentFilter { UserAgentPattern = pattern };
        var sw = new Stopwatch();
        var timings = new List<long>();

        // Act - First call (should compile and cache)
        sw.Start();
        var firstResult = _generator.GetRandomUserAgent(filter);
        sw.Stop();
        var firstCallTime = sw.ElapsedTicks;

        // Subsequent calls (should use cache)
        for (int i = 0; i < 20; i++)
        {
            sw.Restart();
            _ = _generator.GetRandomUserAgent(filter);
            sw.Stop();
            timings.Add(sw.ElapsedTicks);
        }

        // Assert
        Assert.Matches(pattern, firstResult.UserAgent);

        // The first call should take longer (compilation)
        Assert.True(firstCallTime > timings.Average(),
            $"First call ({firstCallTime} ticks) should be slower than subsequent calls (avg: {timings.Average():F2} ticks)");
    }

    [Fact]
    public void RegexPattern_HandlesComplexPatterns()
    {
        // Arrange
        var filter = new UserAgentFilter
        {
            UserAgentPattern = @"Chrome/\d+\.\d+\.\d+\.\d+ Mobile Safari/\d+\.\d+"
        };

        // Act
        var userAgent = _generator.GetRandomUserAgent(filter);

        // Assert
        Assert.Matches(filter.UserAgentPattern, userAgent.UserAgent);
    }

    [Fact]
    public void RegexPattern_ThreadSafety()
    {
        // Arrange
        var patterns = new[]
        {
            @"Chrome/\d+",
            @"Firefox/\d+",
            @"Safari/\d+"
        };

        // Act
        var tasks = new List<Task>();
        var exceptions = new ConcurrentBag<Exception>();

        for (int i = 0; i < 100; i++)
        {
            var pattern = patterns[i % patterns.Length];
            var task = Task.Run(() =>
            {
                try
                {
                    var filter = new UserAgentFilter { UserAgentPattern = pattern };
                    _ = _generator.GetRandomUserAgent(filter);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            });
            tasks.Add(task);
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        Assert.Empty(exceptions);
    }
}
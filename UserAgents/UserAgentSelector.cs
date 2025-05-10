using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using UserAgents.Models;

namespace UserAgents;

public class UserAgentSelector : IUserAgentSelector, IDisposable
{
    private const string EmbeddedUserAgentsFile = "UserAgents.Resources.user_agents.json.gz";
    private const int RegexTimeoutSeconds = 1;
    private readonly IReadOnlyList<UserAgentData> _allUserAgents;
    private readonly ConcurrentDictionary<string, Regex> _regexCache = new();

    public UserAgentSelector()
    {
        _allUserAgents = LoadUserAgents();
    }

    private static IReadOnlyList<UserAgentData> LoadUserAgents()
    {
        var assembly = typeof(UserAgentSelector).Assembly;
        using var stream = assembly.GetManifestResourceStream(EmbeddedUserAgentsFile)
             ?? throw new InvalidOperationException("Could not find embedded resource: user_agents.json.gz");

        using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
        return JsonSerializer.Deserialize<List<UserAgentData>>(gzipStream)
             ?? throw new InvalidOperationException("Failed to deserialize user agents data");
    }

    public UserAgentData GetRandom(bool ignoreWeights = false)
    {
        return GetRandomUserAgentFromList(_allUserAgents, ignoreWeights);
    }

    public UserAgentData GetRandom(UserAgentFilter filters, bool ignoreWeights = false)
    {
        ArgumentNullException.ThrowIfNull(filters);

        var filteredAgents = ApplyFilters(_allUserAgents, filters).ToList();

        if (filteredAgents.Count == 0)
        {
            throw new InvalidOperationException("No user agents match the specified filters");
        }

        return GetRandomUserAgentFromList(filteredAgents, ignoreWeights);
    }

    public void Dispose()
    {
        _regexCache.Clear();
        GC.SuppressFinalize(this);
    }

    private IEnumerable<UserAgentData> ApplyFilters(IReadOnlyList<UserAgentData> userAgents, UserAgentFilter filters)
    {
        var query = userAgents.AsEnumerable();

        if (!string.IsNullOrEmpty(filters.Platform))
        {
            query = query.Where(ua => ua.Platform.Equals(filters.Platform, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(filters.Vendor))
        {
            query = query.Where(ua => ua.Vendor.Equals(filters.Vendor, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(filters.UserAgentPattern))
        {
            var regex = GetOrCreateRegex(filters.UserAgentPattern);
            query = query.Where(ua =>
            {
                try
                {
                    return regex.IsMatch(ua.UserAgent);
                }
                catch (RegexMatchTimeoutException)
                {
                    return false; // Skip entries that timeout
                }
            });
        }

        if (filters.MinScreenWidth.HasValue)
        {
            query = query.Where(ua => ua.ScreenWidth >= filters.MinScreenWidth.Value);
        }

        if (filters.MaxScreenWidth.HasValue)
        {
            query = query.Where(ua => ua.ScreenWidth <= filters.MaxScreenWidth.Value);
        }

        if (filters.MinScreenHeight.HasValue)
        {
            query = query.Where(ua => ua.ScreenHeight >= filters.MinScreenHeight.Value);
        }

        if (filters.MaxScreenHeight.HasValue)
        {
            query = query.Where(ua => ua.ScreenHeight <= filters.MaxScreenHeight.Value);
        }

        if (!string.IsNullOrEmpty(filters.ConnectionType))
        {
            query = query.Where(ua => ua.Connection.Type?.Equals(filters.ConnectionType, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        if (!string.IsNullOrEmpty(filters.EffectiveConnectionType))
        {
            query = query.Where(ua => ua.Connection.EffectiveType.Equals(filters.EffectiveConnectionType, StringComparison.OrdinalIgnoreCase));
        }

        return query;
    }

    private UserAgentData GetRandomUserAgentFromList(IReadOnlyList<UserAgentData> userAgents, bool ignoreWeights)
    {
        if (userAgents == null || userAgents.Count == 0)
        {
            throw new InvalidOperationException("No user agents available");
        }

        if (ignoreWeights)
        {
            return userAgents[Random.Shared.Next(userAgents.Count)];
        }

        var totalWeight = userAgents.Sum(ua => ua.Weight);
        var randomValue = Random.Shared.NextDouble() * totalWeight;

        var currentWeight = 0.0;
        foreach (var userAgent in userAgents)
        {
            currentWeight += userAgent.Weight;
            if (randomValue <= currentWeight)
            {
                return userAgent;
            }
        }

        return userAgents[Random.Shared.Next(userAgents.Count)]; // Fallback
    }

    private Regex GetOrCreateRegex(string pattern)
    {
        return _regexCache.GetOrAdd(pattern, CreateRegexSafely);
    }

    private static Regex CreateRegexSafely(string pattern)
    {
        try
        {
            return new Regex(
                pattern,
                RegexOptions.IgnoreCase | RegexOptions.Compiled,
                TimeSpan.FromSeconds(RegexTimeoutSeconds));
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("Invalid regex pattern", nameof(pattern), ex);
        }
    }
}
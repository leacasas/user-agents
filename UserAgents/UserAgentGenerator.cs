using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using UserAgents.Models;

namespace UserAgents;

public class UserAgentGenerator
{
    private static readonly Lazy<List<UserAgentData>> _userAgents = new(LoadUserAgents);
    private static readonly Random _random = new();
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        MaxDepth = 128
    };

    private static List<UserAgentData> LoadUserAgents()
    {
        var assembly = typeof(UserAgentGenerator).Assembly;
        using var stream = assembly.GetManifestResourceStream("UserAgents.Resources.user_agents.json.gz")
            ?? throw new InvalidOperationException("Could not find embedded resource: user_agents.json.gz");

        using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
        return JsonSerializer.Deserialize<List<UserAgentData>>(gzipStream, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize user agents data");
    }

    public static UserAgentData GetRandomUserAgent(bool ignoreWeights = false)
    {
        var userAgents = _userAgents.Value;
        return GetRandomUserAgentFromList(userAgents, ignoreWeights);
    }

    public static UserAgentData GetRandomUserAgent(UserAgentFilter filters, bool ignoreWeights = false)
    {
        var userAgents = _userAgents.Value;
        var filteredAgents = ApplyFilters(userAgents, filters);

        if (filteredAgents.Count == 0)
        {
            throw new InvalidOperationException("No user agents match the specified filters");
        }

        return GetRandomUserAgentFromList(filteredAgents, ignoreWeights);
    }

    private static UserAgentData GetRandomUserAgentFromList(List<UserAgentData> userAgents, bool ignoreWeights)
    {
        if (ignoreWeights)
        {
            return userAgents[_random.Next(userAgents.Count)];
        }

        var totalWeight = userAgents.Sum(ua => ua.Weight);
        var randomValue = _random.NextDouble() * totalWeight;

        var currentWeight = 0.0;
        foreach (var userAgent in userAgents)
        {
            currentWeight += userAgent.Weight;
            if (randomValue <= currentWeight)
            {
                return userAgent;
            }
        }

        // Fallback to a random user agent if something goes wrong with the weights
        return userAgents[_random.Next(userAgents.Count)];
    }

    private static List<UserAgentData> ApplyFilters(List<UserAgentData> userAgents, UserAgentFilter filters)
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
            query = query.Where(ua => Regex.IsMatch(
                ua.UserAgent,
                filters.UserAgentPattern,
                RegexOptions.IgnoreCase));
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

        return query.ToList();
    }
}
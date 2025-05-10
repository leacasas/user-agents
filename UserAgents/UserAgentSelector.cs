using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace UserAgents;

/// <summary>
/// Implementation of <see cref="IUserAgentSelector"/> that loads user agent data from an embedded resource
/// and provides functionality to select random user agents based on various criteria.
/// </summary>
public class UserAgentSelector : IUserAgentSelector, IDisposable
{
    private const string EmbeddedUserAgentsFile = "UserAgents.Resources.user_agents.json.gz";
    private const int RegexTimeoutSeconds = 1;
    private readonly IReadOnlyList<UserAgentData> _allUserAgents;
    private readonly ConcurrentDictionary<string, Regex> _regexCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAgentSelector"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the embedded user agents data file cannot be found or deserialized.</exception>
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

    #region Public Methods

    /// <summary>
    /// Gets a random user agent from all available user agents.
    /// </summary>
    /// <param name="ignoreWeights">Whether to ignore weights when selecting a random user agent.</param>
    /// <returns>A randomly selected user agent, or null if no user agents are available.</returns>
    public UserAgentData? GetRandom(bool ignoreWeights = false)
    {
        return GetRandomUserAgentFromList(_allUserAgents, ignoreWeights);
    }

    /// <summary>
    /// Gets a random user agent matching the specified filters.
    /// </summary>
    /// <param name="filters">The filters to apply.</param>
    /// <param name="ignoreWeights">Whether to ignore weights when selecting a random user agent.</param>
    /// <returns>A randomly selected user agent matching the filters, or null if no user agents match the filters.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filters"/> is null.</exception>
    public UserAgentData? GetRandom(UserAgentFilter filters, bool ignoreWeights = false)
    {
        ArgumentNullException.ThrowIfNull(filters);

        var filteredAgents = GetFilteredUserAgents(filters);
        return GetRandomUserAgentFromList(filteredAgents, ignoreWeights);
    }

    /// <summary>
    /// Gets multiple random user agents from all available user agents.
    /// </summary>
    /// <param name="count">The number of user agents to get.</param>
    /// <param name="ignoreWeights">Whether to ignore weights when selecting random user agents.</param>
    /// <returns>An enumerable of randomly selected user agents. May contain null values if no user agents are available.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is less than or equal to 0.</exception>
    public IEnumerable<UserAgentData> GetManyRandom(int count, bool ignoreWeights = false)
    {
        ValidateCount(count);
        return GenerateRandomUserAgents(count, _allUserAgents, ignoreWeights);
    }

    /// <summary>
    /// Gets multiple random user agents matching the specified filters.
    /// </summary>
    /// <param name="count">The number of user agents to get.</param>
    /// <param name="filters">The filters to apply.</param>
    /// <param name="ignoreWeights">Whether to ignore weights when selecting random user agents.</param>
    /// <returns>An enumerable of randomly selected user agents matching the filters. May contain null values if no user agents match the filters.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filters"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is less than or equal to 0.</exception>
    public IEnumerable<UserAgentData> GetManyRandom(int count, UserAgentFilter filters, bool ignoreWeights = false)
    {
        ArgumentNullException.ThrowIfNull(filters);
        ValidateCount(count);

        var filteredAgents = GetFilteredUserAgents(filters);
        return GenerateRandomUserAgents(count, filteredAgents, ignoreWeights);
    }

    /// <summary>
    /// Gets all user agents matching the specified filters.
    /// </summary>
    /// <param name="filters">The filters to apply.</param>
    /// <returns>An enumerable of all user agents matching the filters. Returns an empty enumerable if no user agents match the filters.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filters"/> is null.</exception>
    public IEnumerable<UserAgentData> GetAllMatching(UserAgentFilter filters)
    {
        ArgumentNullException.ThrowIfNull(filters);
        return ApplyFilters(_allUserAgents, filters);
    }

    /// <summary>
    /// Disposes of resources used by the selector.
    /// </summary>
    public void Dispose()
    {
        _regexCache.Clear();
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Private Methods

    private static void ValidateCount(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0");
        }
    }

    private IReadOnlyList<UserAgentData> GetFilteredUserAgents(UserAgentFilter filters)
    {
        var filteredAgents = ApplyFilters(_allUserAgents, filters).ToList();

        if (filteredAgents.Count == 0)
        {
            throw new InvalidOperationException("No user agents match the specified filters");
        }

        return filteredAgents;
    }

    private static IEnumerable<UserAgentData> GenerateRandomUserAgents(
        int count,
        IReadOnlyList<UserAgentData> userAgents,
        bool ignoreWeights)
    {
        return Enumerable.Range(0, count)
            .Select(_ => GetRandomUserAgentFromList(userAgents, ignoreWeights))
            .Where(userAgent => userAgent != null)!;
    }

    private static UserAgentData? GetRandomUserAgentFromList(IReadOnlyList<UserAgentData> userAgents, bool ignoreWeights)
    {
        if (userAgents == null || userAgents.Count == 0)
        {
            return null;
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
            currentWeight += userAgent.Weight ?? 0;
            if (randomValue <= currentWeight)
            {
                return userAgent;
            }
        }

        // Fallback
        return userAgents[Random.Shared.Next(userAgents.Count)];
    }

    private IEnumerable<UserAgentData> ApplyFilters(IReadOnlyList<UserAgentData> userAgents, UserAgentFilter filters)
    {
        var query = userAgents.AsEnumerable();

        // Platform filter
        if (!string.IsNullOrEmpty(filters.Platform))
        {
            query = query.Where(ua => ua.Platform.Equals(filters.Platform, StringComparison.OrdinalIgnoreCase));
        }

        // Vendor filter
        if (!string.IsNullOrEmpty(filters.Vendor))
        {
            query = query.Where(ua => ua.Vendor.Equals(filters.Vendor, StringComparison.OrdinalIgnoreCase));
        }

        // UserAgent pattern filter
        if (!string.IsNullOrEmpty(filters.UserAgentPattern))
        {
            query = ApplyUserAgentPatternFilter(query, filters.UserAgentPattern);
        }

        // Screen dimensions filters
        query = ApplyScreenDimensionFilters(query, filters);

        // Connection filters
        query = ApplyConnectionFilters(query, filters);

        return query;
    }

    private IEnumerable<UserAgentData> ApplyUserAgentPatternFilter(IEnumerable<UserAgentData> query, string pattern)
    {
        var regex = GetOrCreateRegex(pattern);
        return query.Where(ua =>
        {
            try
            {
                return regex.IsMatch(ua.UserAgentString);
            }
            catch (RegexMatchTimeoutException)
            {
                return false; // Skip entries that timeout
            }
        });
    }

    private static IEnumerable<UserAgentData> ApplyScreenDimensionFilters(IEnumerable<UserAgentData> query, UserAgentFilter filters)
    {
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

        return query;
    }

    private static IEnumerable<UserAgentData> ApplyConnectionFilters(IEnumerable<UserAgentData> query, UserAgentFilter filters)
    {
        if (!string.IsNullOrEmpty(filters.ConnectionType))
        {
            query = query.Where(ua => ua.Connection?.Type?.Equals(filters.ConnectionType, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        if (!string.IsNullOrEmpty(filters.EffectiveConnectionType))
        {
            query = query.Where(ua => ua.Connection?.EffectiveType?.Equals(filters.EffectiveConnectionType, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        return query;
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

    #endregion
}
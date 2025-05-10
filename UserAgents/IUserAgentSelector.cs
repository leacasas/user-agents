using UserAgents.Models;

namespace UserAgents;

/// <summary>
/// Provides functionality to select random user agents from an embedded dataset.
/// </summary>
public interface IUserAgentSelector
{
    /// <summary>
    /// Gets a random user agent from the embedded dataset.
    /// </summary>
    /// <param name="ignoreWeights">When true, ignores the weight distribution of user agents and selects completely randomly.</param>
    /// <returns>A randomly selected user agent data object.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no user agents are available.</exception>
    UserAgentData GetRandom(bool ignoreWeights = false);

    /// <summary>
    /// Gets a random user agent that matches the specified filters.
    /// </summary>
    /// <param name="filters">The filters to apply when selecting a user agent.</param>
    /// <param name="ignoreWeights">When true, ignores the weight distribution of user agents and selects completely randomly.</param>
    /// <returns>A randomly selected user agent data object that matches the specified filters.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filters"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no user agents match the specified filters.</exception>
    /// <remarks>
    /// The following filter criteria are supported:
    /// <list type="bullet">
    /// <item><description>Platform (e.g., "iPhone", "Windows")</description></item>
    /// <item><description>Vendor (e.g., "Apple Computer, Inc.")</description></item>
    /// <item><description>User agent pattern (regex)</description></item>
    /// <item><description>Screen dimensions (min/max width and height)</description></item>
    /// <item><description>Connection type (e.g., "wifi")</description></item>
    /// <item><description>Effective connection type</description></item>
    /// </list>
    /// </remarks>
    UserAgentData GetRandom(UserAgentFilter filters, bool ignoreWeights = false);
}

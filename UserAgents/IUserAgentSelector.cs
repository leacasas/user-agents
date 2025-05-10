using UserAgents.Models;

namespace UserAgents;

public interface IUserAgentSelector
{
    UserAgentData GetRandom(bool ignoreWeights = false);
    UserAgentData GetRandom(UserAgentFilter filters, bool ignoreWeights = false);
}

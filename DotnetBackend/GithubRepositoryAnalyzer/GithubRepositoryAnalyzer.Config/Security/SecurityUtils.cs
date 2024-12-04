using GithubRepositoryAnalyzer.Config.Extensions;

namespace GithubRepositoryAnalyzer.Config.Security;

public static class SecurityUtils
{
    public static bool IsValid(string info, string salt, string hash, ISecurityHashEngine hashEngine)
    {

        var tempHash = hashEngine.GenerateHash(info, salt);

        return tempHash.EqualsIgnoreCase(hash);
    }
}
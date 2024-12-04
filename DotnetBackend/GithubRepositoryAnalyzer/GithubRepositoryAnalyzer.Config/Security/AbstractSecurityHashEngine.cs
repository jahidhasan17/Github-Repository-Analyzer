using System.Security.Cryptography;
using GithubRepositoryAnalyzer.Config.Extensions;

namespace GithubRepositoryAnalyzer.Config.Security;

public abstract class AbstractSecurityHashEngine : ISecurityHashEngine
{
    public virtual string GenerateSalt()
    {
        var buffer = new byte[0x10];
        RandomNumberGenerator.Create().GetBytes(buffer);

        return Convert.ToBase64String(buffer);
    }

    public virtual string GenerateSalt(string salt)
    {

        return Convert.ToBase64String(salt.ToByteArray());
    }

    public abstract string GenerateHash(string info, string salt);
}
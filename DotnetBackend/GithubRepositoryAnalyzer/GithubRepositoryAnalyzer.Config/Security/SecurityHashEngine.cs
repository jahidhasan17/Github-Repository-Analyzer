using System.Security.Cryptography;
using GithubRepositoryAnalyzer.Config.Extensions;

namespace GithubRepositoryAnalyzer.Config.Security;

public class SecurityHashEngine : AbstractSecurityHashEngine
{
    private readonly HashAlgorithm hashAlgorithm;

    public SecurityHashEngine(HashAlgorithm hashAlgorithm)
    {
        this.hashAlgorithm = hashAlgorithm;
    }

    public static ISecurityHashEngine CreateSha256()
    {
        return new SecurityHashEngine(SHA256.Create());
    }

    public override string GenerateHash(string info, string salt)
    {
        var buffer1 = System.Text.Encoding.UTF8.GetBytes(salt);
        var buffer2 = ArrayX.MergeByteArrays(info.ToByteArray(), buffer1);
        var buffer3 = hashAlgorithm.ComputeHash(buffer2);

        return Convert.ToBase64String(buffer3);
    }
}
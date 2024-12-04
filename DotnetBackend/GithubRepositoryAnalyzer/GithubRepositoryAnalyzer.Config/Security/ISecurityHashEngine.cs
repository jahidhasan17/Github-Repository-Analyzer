namespace GithubRepositoryAnalyzer.Config.Security;

public interface ISecurityHashEngine
{
    string GenerateSalt();

    string GenerateSalt(string salt);

    string GenerateHash(string info, string salt);
}
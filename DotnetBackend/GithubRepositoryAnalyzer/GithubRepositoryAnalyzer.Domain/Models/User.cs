using GithubRepositoryAnalyzer.Config.Security;

namespace GithubRepositoryAnalyzer.Domain.Models;

public class User : Entity
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<UserToken> UserTokens { get; set; } = new HashSet<UserToken>();
    
    public ISecurityHashEngine HashEngine { get; } = SecurityHashEngine.CreateSha256();
}
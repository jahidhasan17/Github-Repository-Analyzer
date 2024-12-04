namespace GithubRepositoryAnalyzer.Domain.Models;

public class UserToken : Entity
{
    public string UserId { get; set; }

    public string RefreshToken { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public User User { get; set; }
}
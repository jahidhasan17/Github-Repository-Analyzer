using System.Security.Claims;
using GithubRepositoryAnalyzer.Domain.Models;

namespace GithubRepositoryAnalyzer.Services;

public interface ITokenService
{
    public string GenerateAccessToken(User user);

    public string GenerateRefreshToken();

    public ClaimsPrincipal? GetClaimsFromToken(string token);
}
using GithubRepositoryAnalyzer.Config.Security;
using GithubRepositoryAnalyzer.Domain.Context;
using GithubRepositoryAnalyzer.Domain.Models;
using GithubRepositoryAnalyzer.Dto;
using GithubRepositoryAnalyzer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GithubRepositoryAnalyzer.Controller;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly ConfigContext ConfigContext;
    private readonly ITokenService TokenService;

    public IdentityController(ConfigContext configContext, ITokenService tokenService)
    {
        ConfigContext = configContext;
        TokenService = tokenService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequest request)
    {
        var validation = ValidationService.ValidateSignup(request);
        if (!validation.IsValid)
        {
            return BadRequest(validation.ErrorMessage);
        }

        var existingUser = await ConfigContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            return Conflict("User with given username already exists.");
        }


        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.UserName,
            Email = request.Email,
        };
        
        user.Password = user.HashEngine.GenerateHash(request.Password, "mySalt");
        
        await ConfigContext.Users.AddAsync(user);
        await ConfigContext.SaveChangesAsync();

        return CreatedAtAction(nameof(Signup), new { user.Id }, new { message = "Account created successfully" });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var validation = ValidationService.ValidateLogin(request);
        if (!validation.IsValid)
        {
            return BadRequest(validation.ErrorMessage);
        }

        var user = await ConfigContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }
        
        if (!SecurityUtils.IsValid(request.Password, "mySalt", user.Password, user.HashEngine))
        {
            return Unauthorized("Invalid username or password.");
        }

        var accessToken = TokenService.GenerateAccessToken(user);
        var refreshToken = TokenService.GenerateRefreshToken();

        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(10);

        var userToken = new UserToken
        {
            Id = Guid.NewGuid().ToString(),
            UserId = user.Id,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = refreshTokenExpiryTime
        };

        await ConfigContext.UserTokens.AddAsync(userToken);
        await ConfigContext.SaveChangesAsync();

        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Message = "Logged in successfully"
        });
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenRefreshRequest request)
    {
        var claimsPrincipal = TokenService.GetClaimsFromToken(request.AccessToken);
        if (claimsPrincipal == null || claimsPrincipal.Identity == null)
        {
            return BadRequest("Invalid AccessToken");
        }

        var userName = claimsPrincipal.Identity.Name;
        var user = await ConfigContext.Users.FirstOrDefaultAsync(u => u.Name == userName);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        var userToken = await ConfigContext.UserTokens.FirstOrDefaultAsync(t => t.UserId == user.Id && t.RefreshToken == request.RefreshToken);
        if (userToken == null ||
            userToken.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid RefreshToken");
        }

        var newAccessToken = TokenService.GenerateAccessToken(user);

        var newRefreshToken = TokenService.GenerateRefreshToken();
        userToken.RefreshToken = newRefreshToken;
        userToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(10);
        
        ConfigContext.UserTokens.Update(userToken);
        await ConfigContext.SaveChangesAsync();
        
        return Ok(new
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Message = "Tokens refreshed successfully"
        });
    }
}
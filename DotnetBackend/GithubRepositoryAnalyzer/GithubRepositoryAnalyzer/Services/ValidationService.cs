using GithubRepositoryAnalyzer.Dto;

namespace GithubRepositoryAnalyzer.Services;

public class ValidationService
{
    public static (bool IsValid, string ErrorMessage) ValidateSignup(SignupRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Email))
            return (false, "All fields are required.");

        if (request.Password.Length < 6)
            return (false, "Password must be at least 6 characters long.");

        return (true, string.Empty);
    }

    public static (bool IsValid, string ErrorMessage) ValidateLogin(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return (false, "Username and password are required.");

        return (true, string.Empty);
    }
}
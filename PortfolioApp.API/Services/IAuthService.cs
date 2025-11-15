namespace PortfolioApp.API.Services;

public interface IAuthService
{
    bool ValidateCredentials(string username, string password);
    string GenerateJwtToken(string username);
}

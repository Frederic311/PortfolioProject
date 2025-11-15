using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace PortfolioApp.Tests.Helpers;

public class AuthHelper
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;

    public AuthHelper(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    public async Task<string> GetAuthTokenAsync()
    {
        // ✅ Utiliser le même mot de passe que dans les user-secrets (admin124)
        var loginDto = new
        {
            Username = "admin",
        Password = "admin124"  // ✅ CORRIGÉ : correspond maintenant au hash configuré
        };

  try
{
     var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
 
            if (!response.IsSuccessStatusCode)
 {
                var error = await response.Content.ReadAsStringAsync();
    throw new Exception($"Login failed: {response.StatusCode} - {error}");
  }

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
     return result?.Token ?? throw new Exception("Token is null");
        }
      catch (Exception ex)
        {
     throw new Exception($"Failed to get auth token: {ex.Message}", ex);
  }
    }

    public async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
  new AuthenticationHeaderValue("Bearer", token);
        return _client;
    }
}

// ✅ DTO pour le login - correspond à la réponse de l'API
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }  // ✅ CORRIGÉ : ExpiresAt au lieu de Username
}

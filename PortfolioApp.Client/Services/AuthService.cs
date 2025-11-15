using System.Net.Http.Json;
using PortfolioApp.Shared.DTOs;
using Microsoft.JSInterop;

namespace PortfolioApp.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly ITokenService _tokenService;
    private const string TokenKey = "authToken";
    private const string ExpiryKey = "authExpiry";

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, ITokenService tokenService)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _tokenService = tokenService;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var loginRequest = new LoginRequestDto
            {
                Username = username,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                if (loginResponse != null)
                {
                    // Store token using TokenService AND localStorage
                    await _tokenService.SetTokenAsync(loginResponse.Token);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ExpiryKey, loginResponse.ExpiresAt.ToString("o"));

                    // Set authorization header
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.Token);

                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _tokenService.ClearTokenAsync();
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", ExpiryKey);
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        catch
        {
            // Ignore errors during logout
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var expiryString = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", ExpiryKey);
            if (string.IsNullOrEmpty(expiryString))
            {
                return false;
            }

            if (DateTime.TryParse(expiryString, out var expiry))
            {
                if (expiry > DateTime.UtcNow)
                {
                    // Token still valid, set authorization header
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    return true;
                }
            }

            // Token expired or invalid, clean up
            await LogoutAsync();
            return false;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
        {
            // Try to use cached token during prerendering
            var token = await _tokenService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _tokenService.GetTokenAsync();
    }
}

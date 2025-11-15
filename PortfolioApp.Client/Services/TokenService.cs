using Microsoft.JSInterop;

namespace PortfolioApp.Client.Services;

public interface ITokenService
{
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task ClearTokenAsync();
}

public class TokenService : ITokenService
{
    private readonly IJSRuntime _jsRuntime;
    private const string TokenKey = "authToken";

    public TokenService(IJSRuntime jsRuntime)
    {
  _jsRuntime = jsRuntime;
    }

    public async Task<string?> GetTokenAsync()
    {
   try
     {
          var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
     return token;
        }
   catch (Exception ex)
    {
    Console.WriteLine($"❌ Error getting token: {ex.Message}");
   return null;
    }
    }

    public async Task SetTokenAsync(string token)
    {
  try
  {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
            Console.WriteLine("✅ Token stored in localStorage");
        }
        catch (Exception ex)
        {
  Console.WriteLine($"❌ Error setting token: {ex.Message}");
     }
    }

    public async Task ClearTokenAsync()
    {
        try
     {
 await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            Console.WriteLine("✅ Token removed from localStorage");
        }
     catch (Exception ex)
     {
        Console.WriteLine($"❌ Error clearing token: {ex.Message}");
     }
    }
}

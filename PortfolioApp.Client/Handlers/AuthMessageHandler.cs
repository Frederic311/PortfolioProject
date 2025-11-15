using System.Net.Http.Headers;
using PortfolioApp.Client.Services;

namespace PortfolioApp.Client.Handlers;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public AuthMessageHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Try to get token from TokenService (which handles caching and prerendering)
            var token = await _tokenService.GetTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                // Attach token to request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine($"✅ Token attached to {request.Method} {request.RequestUri?.PathAndQuery}");
            }
            else
            {
                Console.WriteLine($"⚠️ No token available for {request.Method} {request.RequestUri?.PathAndQuery}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in AuthMessageHandler: {ex.Message}");
            // Don't throw - let the request proceed and let the API handle auth
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

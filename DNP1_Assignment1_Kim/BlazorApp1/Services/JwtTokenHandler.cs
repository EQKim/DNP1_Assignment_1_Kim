using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;

public class JwtTokenHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorageService;

    public JwtTokenHandler(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _localStorageService.GetItemAsync<string>("ThisIsASuperSecureSecretKey32Char");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (InvalidOperationException)
        {
            // This will catch the exception when trying to access JS Interop during pre-rendering.
            // We'll continue without setting the authorization header.
            // Once client-side rendering starts, the token will be added to subsequent requests.
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
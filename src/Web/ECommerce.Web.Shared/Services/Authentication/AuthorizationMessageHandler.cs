using System.Net.Http.Headers;

namespace ECommerce.Web.Shared.Services.Authentication;

internal sealed class AuthorizationMessageHandler(TokenStore tokenStore) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (tokenStore.Tokens?.AccessToken is not null)
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenStore.Tokens.AccessToken);
        }

        return base.SendAsync(request, cancellationToken);
    }
}

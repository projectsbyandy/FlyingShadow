using System.Net.Http.Headers;

namespace FlyingShadow.Api.Spec.Tests.Support;

public sealed class BearerTokenHandler : DelegatingHandler
{
    private readonly string? _token;

    public BearerTokenHandler(string? token)
    {
        _token = token;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_token) is false)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
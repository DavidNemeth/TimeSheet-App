using System.Net.Http.Headers;
using TimeSheet.Api.Services;

namespace TimeSheet.Api
{
    public class AuthenticationMessageHandler : DelegatingHandler
    {
        private readonly ITokenService _tokenService;

        public AuthenticationMessageHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await _tokenService.GetAccessTokenAsync();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}

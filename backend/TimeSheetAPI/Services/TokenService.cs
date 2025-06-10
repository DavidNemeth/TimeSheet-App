using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

namespace TimeSheet.Api.Services
{
    public interface ITokenService
    {
        Task<string> GetAccessTokenAsync();
    }
    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly SemaphoreSlim _tokenSemaphore = new SemaphoreSlim(1, 1);
        private string _accessToken;
        private DateTime _accessTokenExpiration = DateTime.MinValue;

        public TokenService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _accessTokenExpiration)
            {
                await _tokenSemaphore.WaitAsync();
                try
                {
                    if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _accessTokenExpiration)
                    {
                        await RequestNewTokenAsync();
                    }
                }
                finally
                {
                    _tokenSemaphore.Release();
                }
            }
            return _accessToken;
        }

        private async Task RequestNewTokenAsync()
        {
            var clientId = _configuration["TimesheetJWT:Id"];
            var clientSecret = _configuration["TimesheetJWT:ClientSecretHash"];
            var tokenEndpoint = _configuration["BaseApiUrl"] + "/api/token";

            var requestBody = new
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };

            var response = await _httpClient.PostAsJsonAsync(tokenEndpoint, requestBody);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

                _accessToken = tokenResponse.AccessToken;

                // Parse the token to get expiration time
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(_accessToken);

                _accessTokenExpiration = jwtToken.ValidTo.AddSeconds(-60); // Renew token 1 minute before expiration
            }
            else
            {
                throw new Exception($"Unable to obtain access token. Status code: {response.StatusCode}");
            }
        }
    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}

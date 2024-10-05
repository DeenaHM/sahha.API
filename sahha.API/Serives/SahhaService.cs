using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using sahha.API.Entities;
using sahha.API.Helpers;
using System.Net.Http.Headers;
using System.Text.Json;

namespace sahha.API.Serives
{
    public class SahhaService : ISahhaService
    {
        private readonly SahhaApiSettings _sahhaApiSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SahhaService> _logger;

        public SahhaService(IOptions<SahhaApiSettings> sahhaApiOptions, HttpClient httpClient, ILogger<SahhaService> logger)
        {
            _sahhaApiSettings = sahhaApiOptions.Value ?? throw new ArgumentNullException(nameof(sahhaApiOptions));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetTokenAsync()
        {
            try
            {
                var requestBody = new
                {
                    clientId = _sahhaApiSettings.ClientId,
                    clientSecret = _sahhaApiSettings.ClientSecret
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, _sahhaApiSettings.TokenUrl)
                {
                    Content = content
                };

                _logger.LogInformation("Requesting token from {TokenUrl}", _sahhaApiSettings.TokenUrl);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                if (tokenResponse is null || string.IsNullOrEmpty(tokenResponse.accountToken))
                {
                    _logger.LogError("Failed to retrieve account token.");
                    throw new Exception("Token response is null or empty.");
                }

                _logger.LogInformation("Successfully retrieved token.");
                return tokenResponse.accountToken;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error occurred while requesting token.");
                throw;
            }
        }

        public async Task<ProfileTokenResponse?> RegisterProfileAsync(Guid externalId)
        {
            try
            {
                var token = await GetTokenAsync();

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_sahhaApiSettings.BaseUrl}oauth/profile/register");
                request.Headers.Authorization = new AuthenticationHeaderValue("account", token);

                var requestBody = new { externalId = externalId };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                request.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                _logger.LogInformation("Registering profile with externalId: {ExternalId}", externalId);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var profileTokenResponse = JsonSerializer.Deserialize<ProfileTokenResponse>(responseContent);

                _logger.LogInformation("Successfully registered profile.");
                return profileTokenResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error occurred while registering profile.");
                throw;
            }
        }

        public async Task<ProfileTokenResponse?> GetProfileTokenAsync(Guid externalId)
        {
            try
            {
                var accountToken = await GetTokenAsync();

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_sahhaApiSettings.BaseUrl}oauth/profile/token");
                request.Headers.Authorization = new AuthenticationHeaderValue("profile", accountToken);

                var requestBody = new { externalId = externalId };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                request.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                _logger.LogInformation("Getting profile token for externalId: {ExternalId}", externalId);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var profileTokenResponse = JsonSerializer.Deserialize<ProfileTokenResponse>(responseContent);

                _logger.LogInformation("Successfully retrieved profile token.");
                return profileTokenResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving profile token.");
                throw;
            }
        }


        public async Task<List<BiomarkerData>> GetBiomarkersAsync(Guid externalId, string categories, string types, DateTime startDateTime, DateTime endDateTime)
        {
            try
            {
                // var profileToken = await GetTokenAsync();
                var profileToken = await GetProfileTokenAsync(externalId);

                var requestUrl = $"{_sahhaApiSettings.BaseUrl}profile/biomarker?categories={categories}&types={types}&startDateTime={startDateTime:yyyy-MM-ddTHH:mm:ssZ}&endDateTime={endDateTime:yyyy-MM-ddTHH:mm:ssZ}";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("profile", profileToken.ProfileToken);

                _logger.LogInformation("Requesting biomarkers data from {Url}", requestUrl);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Successfully retrieved biomarkers data.");
                _logger.LogInformation("Response content: {Response}", responseContent);

                var biomarkers = JsonSerializer.Deserialize< List<BiomarkerData>>(responseContent);
                var sortedBiomarkers = biomarkers.OrderByDescending(x => x.StartDateTime).ToList();
                return sortedBiomarkers;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving biomarkers.");
                throw;
            }
        }

    }
}

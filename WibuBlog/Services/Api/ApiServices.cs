using Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services.Api
{
    public class ApiServices(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IOptions<AuthTokenOptions> authTokenOptions) : IApiServices
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("api");
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var client = CreateClient();
            AddAuthorizationHeader(ref client);
            var response = await client.GetAsync(endpoint);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var client = CreateClient();
            AddAuthorizationHeader(ref client);
            var response = await client.PostAsJsonAsync(endpoint, data);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            var client = CreateClient();
            AddAuthorizationHeader(ref client);
            var response = await client.PutAsJsonAsync(endpoint, data);
            return await HandleResponse<T>(response);
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var client = CreateClient();
            AddAuthorizationHeader(ref client);
            var response = await client.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request failed: {response.StatusCode}");
            }

            var jsonResponse = await response.Content.ReadFromJsonAsync<T>();
            return jsonResponse ?? throw new InvalidOperationException("Response content is null");
        }

        private void AddAuthorizationHeader(ref HttpClient client)
        {
            var authToken = _httpContextAccessor.HttpContext?.Request.Cookies[_authTokenOptions.Name];

            if (!string.IsNullOrEmpty(authToken) && client.DefaultRequestHeaders.Authorization == null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }
        }
    }
}
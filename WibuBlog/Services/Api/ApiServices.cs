using Infrastructure.Extensions;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services.Api
{
    public class ApiServices(IHttpClientFactory httpClientFactory) : IApiServices
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("api");
        }
            
        public async Task<T> GetAsync<T>(string endpoint)
        {
            var client = CreateClient();
            var response = await client.GetAsync(endpoint);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(endpoint, data);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            var client = CreateClient();
            var response = await client.PutAsJsonAsync(endpoint, data);
            return await HandleResponse<T>(response);
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request failed: {response.StatusCode}");
            }
            var json = await response.Content.ReadAsStringAsync();
            return DeserializeExtensions.Deserialize<T>(json);
        }
    }
}
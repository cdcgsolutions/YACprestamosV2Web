using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WEB1.Module.CLIENT.RN
{
    /// <summary>
    /// Servicio cliente para comunicación con la API del backend.
    /// </summary>
    public class DataMasterApi
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;
        private readonly IConfiguration _configuration;

        public DataMasterApi(IHttpClientFactory httpClientFactory, IJSRuntime js, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("BackendApi");
            _js = js;
            _configuration = configuration;
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            try
            {
                var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch
            {
                // Ignorar durante prerendering
            }
        }

        public async Task<(List<string>? Data, string? Error)> GetExampleData()
        {
            await SetAuthorizationHeaderAsync();

            // Simulación demostrativa:
            await Task.Delay(300);
            return (new List<string> { "Elemento 1", "Elemento 2", "Elemento 3" }, null);
        }
    }
}

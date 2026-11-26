using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using WEB1.SandBox.Authentication;
using WEB1.SandBox.DTOs;

namespace WEB1.SandBox.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IConfiguration _configuration;

        public AuthService(
            IHttpClientFactory httpClientFactory,
            AuthenticationStateProvider authStateProvider,
            IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("BackendApi");
            _authStateProvider = authStateProvider;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message)> LoginAsync(LoginDTO loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginModel);
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDTO>>();

                if (response.IsSuccessStatusCode && result != null && result.IsSuccess && result.Value != null)
                {
                    if (_authStateProvider is CustomAuthStateProvider customProvider)
                    {
                        await customProvider.MarkUserAsAuthenticated(result.Value.Token);
                    }
                    return (true, result.Message ?? "Inicio de sesión exitoso.");
                }

                var errorMsg = result?.Message ?? "Credenciales inválidas o error en el servidor.";
                if (result?.Errors != null && result.Errors.Count > 0)
                {
                    errorMsg += $" ({string.Join(", ", result.Errors)})";
                }

                return (false, errorMsg);
            }
            catch (Exception ex)
            {
                return (false, $"Error al conectar con el servidor: {ex.Message}");
            }
        }

        public async Task LogoutAsync()
        {
            if (_authStateProvider is CustomAuthStateProvider customProvider)
            {
                await customProvider.MarkUserAsLoggedOut();
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            if (_authStateProvider is CustomAuthStateProvider customProvider)
            {
                return await customProvider.GetTokenAsync();
            }
            return null;
        }

        public async Task<(bool Success, string Message)> CambiarContrasenaAsync(CambiarContrasenaDTO model)
        {
            try
            {
                var token = await GetTokenAsync();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.PostAsJsonAsync("api/Auth/cambiar-contrasena", model);
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDTO>>();

                if (response.IsSuccessStatusCode && result != null && result.IsSuccess && result.Value != null)
                {
                    if (_authStateProvider is CustomAuthStateProvider customProvider)
                    {
                        await customProvider.MarkUserAsAuthenticated(result.Value.Token);
                    }
                    return (true, result.Message ?? "Contraseña actualizada exitosamente.");
                }

                var errorMsg = result?.Message ?? "No se pudo actualizar la contraseña.";
                if (result?.Errors != null && result.Errors.Count > 0)
                {
                    errorMsg += $" ({string.Join(", ", result.Errors)})";
                }

                return (false, errorMsg);
            }
            catch (Exception ex)
            {
                return (false, $"Error al conectar con el servidor: {ex.Message}");
            }
        }
    }
}

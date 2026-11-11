using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WEB1.SandBox.Authentication;
using WEB1.SandBox.DTOs;

namespace WEB1.SandBox.Services
{
    public class ModuleService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<ModuleService>? _logger;

        private bool _hasLoadedFromApi = false;

        public List<ModuleDTO> Modules { get; private set; } = new();
        public event Action? OnModulesChanged;

        public bool IsPlantilla
        {
            get
            {
                if (bool.TryParse(_configuration["app:Plantilla"], out var appPlantilla))
                    return appPlantilla;
                if (bool.TryParse(_configuration["Plantilla"], out var rootPlantilla))
                    return rootPlantilla;
                return true; // Por defecto true si no está especificado
            }
        }

        public ModuleService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            AuthenticationStateProvider authStateProvider,
            ILogger<ModuleService>? logger = null)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _authStateProvider = authStateProvider;
            _logger = logger;

            if (IsPlantilla)
            {
                LoadMockModules();
            }
            else
            {
                Modules = new List<ModuleDTO>();
            }
        }

        public async Task<List<ModuleDTO>> GetModulesAsync(bool forceRefresh = false)
        {
            if (IsPlantilla)
            {
                if (Modules.Count == 0 || forceRefresh)
                {
                    LoadMockModules();
                }
                return Modules;
            }

            if (_hasLoadedFromApi && !forceRefresh)
            {
                return Modules;
            }

            try
            {
                var client = _httpClientFactory.CreateClient("BackendApi");

                if (_authStateProvider is CustomAuthStateProvider customProvider)
                {
                    var token = await customProvider.GetTokenAsync();
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }

                var endpoint = _configuration["Apis:ModulesEndpoint"] ?? "api/Modules";
                var response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    List<ModuleDTO>? loaded = null;
                    try
                    {
                        var apiResp = await response.Content.ReadFromJsonAsync<ApiResponse<List<ModuleDTO>>>();
                        if (apiResp != null && apiResp.IsSuccess && apiResp.Value != null)
                        {
                            loaded = apiResp.Value;
                        }
                    }
                    catch
                    {
                        loaded = await response.Content.ReadFromJsonAsync<List<ModuleDTO>>();
                    }

                    Modules = loaded ?? new List<ModuleDTO>();
                    _hasLoadedFromApi = true;
                    OnModulesChanged?.Invoke();
                    return Modules;
                }
                else
                {
                    _logger?.LogWarning("No se pudieron obtener los módulos desde la API. Status Code: {StatusCode}", response.StatusCode);
                    Modules = new List<ModuleDTO>();
                    _hasLoadedFromApi = true;
                    OnModulesChanged?.Invoke();
                    return Modules;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al consultar los módulos desde la API.");
                Modules = new List<ModuleDTO>();
                _hasLoadedFromApi = true;
                OnModulesChanged?.Invoke();
                return Modules;
            }
        }

        public async Task<ModuleDTO?> GetModuleByPathAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            if (Modules.Count == 0)
            {
                await GetModulesAsync();
            }

            return GetModuleByPath(path);
        }

        public ModuleDTO? GetModuleByPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            var clean = path.Trim('/');
            return Modules.FirstOrDefault(m => m.path.Trim('/').Equals(clean, StringComparison.OrdinalIgnoreCase));
        }

        public void LoadMockModules()
        {
            var demoInterfaces = new List<InterfaceDTO>
            {
                new() 
                { 
                    name = "Showcase Componentes", 
                    description = "Explora los componentes UI de MudBlazor (Botones, Tablas, Formularios, Diálogos).", 
                    path = "examples/components", 
                    icon = "bi bi-layers", 
                    status = true 
                },
                new() 
                { 
                    name = "Ejemplo HTTP", 
                    description = "Demostración de consumo de servicios API y peticiones HTTP autenticadas.", 
                    path = "examples/http", 
                    icon = "bi bi-globe", 
                    status = true 
                },
                new() 
                { 
                    name = "Utilidades", 
                    description = "Pruebas de FloatingMessage, FocusedMessage (SweetAlert2) y servicios de diseño.", 
                    path = "examples/utilities", 
                    icon = "bi bi-tools", 
                    status = true 
                }
            };

            Modules = new List<ModuleDTO>
            {
                new()
                {
                    name = "Módulo de Ejemplos",
                    description = "Conjunto de pantallas de prueba para el desarrollo con MudBlazor y arquitectura modular.",
                    path = "examples",
                    icon = "bi bi-code-square",
                    status = true,
                    interfaceList = demoInterfaces
                }
            };

            OnModulesChanged?.Invoke();
        }
    }
}

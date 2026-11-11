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
                return true;
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

                    if (loaded != null && loaded.Any())
                    {
                        Modules = loaded;
                        _hasLoadedFromApi = true;
                        OnModulesChanged?.Invoke();
                        return Modules;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error al cargar módulos desde la API. Usando módulos locales.");
            }

            LoadMockModules();
            return Modules;
        }

        public async Task<ModuleDTO?> GetModuleByPathAsync(string path)
        {
            if (Modules.Count == 0)
            {
                await GetModulesAsync();
            }

            if (string.IsNullOrWhiteSpace(path)) return null;
            var clean = path.Trim('/');
            return Modules.FirstOrDefault(m => m.path.Trim('/').Equals(clean, StringComparison.OrdinalIgnoreCase));
        }

        public void LoadMockModules()
        {
            var interfaces = new List<InterfaceDTO>
            {
                new() 
                { 
                    name = "Clientes", 
                    description = "Gestión de prestatarios, datos de contacto y estado.", 
                    path = "clientes", 
                    icon = "fa-solid fa-users", 
                    status = true 
                },
                new() 
                { 
                    name = "Préstamos", 
                    description = "Otorgamiento de préstamos, cálculo de cuotas y registro de abonos.", 
                    path = "prestamos", 
                    icon = "fa-solid fa-sack-dollar", 
                    status = true 
                },
                new() 
                { 
                    name = "Usuarios", 
                    description = "Control de accesos, roles de operadores y contraseñas.", 
                    path = "usuarios", 
                    icon = "fa-solid fa-user-gear", 
                    status = true 
                },
                new() 
                { 
                    name = "Dashboard", 
                    description = "Métricas financieras, gráficos interactivos y exportación de reportes.", 
                    path = "dashboard", 
                    icon = "fa-solid fa-chart-pie", 
                    status = true 
                }
            };

            Modules = new List<ModuleDTO>
            {
                new()
                {
                    name = "Y.A.C. Prestamos",
                    description = "Módulo principal para control de clientes, préstamos y usuarios.",
                    path = "operaciones",
                    icon = "fa-solid fa-hand-holding-dollar",
                    status = true,
                    interfaceList = interfaces
                }
            };

            OnModulesChanged?.Invoke();
        }
    }
}

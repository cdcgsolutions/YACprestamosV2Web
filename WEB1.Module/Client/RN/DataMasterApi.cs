using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WEB1.Module.DTOs;

namespace WEB1.Module.CLIENT.RN
{
    public class DataMasterApi
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public DataMasterApi(IHttpClientFactory httpClientFactory, IJSRuntime js)
        {
            _httpClient = httpClientFactory.CreateClient("BackendApi");
            _js = js;
        }

        private async Task SetTokenAsync()
        {
            try
            {
                var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch { }
        }

        #region Helpers HTTP Nativos y Seguros
        private async Task<Result<T>> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    return new Result<T>
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Error HTTP {(int)response.StatusCode} ({response.ReasonPhrase})."
                    };
                }
                return new Result<T> { IsSuccess = false, ErrorMessage = "Respuesta vacía del servidor." };
            }

            try
            {
                var result = JsonSerializer.Deserialize<Result<T>>(content, _jsonOptions);
                if (result != null)
                {
                    return result;
                }
            }
            catch
            {
                if (!response.IsSuccessStatusCode)
                {
                    return new Result<T>
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Error {(int)response.StatusCode}: {content}"
                    };
                }
            }

            return new Result<T> { IsSuccess = false, ErrorMessage = "Error al procesar la respuesta del servidor." };
        }

        private async Task<Result<T>> SendGetAsync<T>(string endpoint)
        {
            try
            {
                await SetTokenAsync();
                var response = await _httpClient.GetAsync(endpoint);
                return await ProcessResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                return new Result<T> { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        private async Task<Result<T>> SendPostAsync<T>(string endpoint, object data)
        {
            try
            {
                await SetTokenAsync();
                var jsonPayload = JsonSerializer.Serialize(data, _jsonOptions);
                using var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, httpContent);
                return await ProcessResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                return new Result<T> { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        private async Task<Result<T>> SendPutAsync<T>(string endpoint, object data)
        {
            try
            {
                await SetTokenAsync();
                var jsonPayload = JsonSerializer.Serialize(data, _jsonOptions);
                using var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(endpoint, httpContent);
                return await ProcessResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                return new Result<T> { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        private async Task<Result<T>> SendPatchAsync<T>(string endpoint)
        {
            try
            {
                await SetTokenAsync();
                var response = await _httpClient.PatchAsync(endpoint, null);
                return await ProcessResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                return new Result<T> { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        #endregion

        #region CLIENTES
        public Task<Result<List<ClienteDTO>>> GetClientesAsync() => 
            SendGetAsync<List<ClienteDTO>>("api/Cliente");

        public Task<Result<ClienteDTO>> GetClienteByIdAsync(int id) => 
            SendGetAsync<ClienteDTO>($"api/Cliente/{id}");

        public Task<Result<ClienteDTO>> CrearClienteAsync(ClienteDTO dto) => 
            SendPostAsync<ClienteDTO>("api/Cliente/Creacion", dto);

        public Task<Result<ClienteDTO>> ActualizarClienteAsync(int id, ClienteDTO dto)
        {
            if (dto.IdCliente == 0) dto.IdCliente = id;
            return SendPutAsync<ClienteDTO>("api/Cliente/Edicion", dto);
        }

        public Task<Result<ClienteDTO>> CambiarEstadoClienteAsync(int id) => 
            SendPutAsync<ClienteDTO>($"api/Cliente/CambiarEstado/{id}", new { });
        #endregion

        #region PRESTAMOS
        public Task<Result<List<PrestamoDTO>>> GetPrestamosAsync() => 
            SendGetAsync<List<PrestamoDTO>>("api/Prestamo");

        public Task<Result<List<PrestamoDTO>>> GetPrestamosPorClienteAsync(int idCliente) => 
            SendGetAsync<List<PrestamoDTO>>($"api/Prestamo/PorCliente/{idCliente}");

        public Task<Result<PrestamoDTO>> CrearPrestamoAsync(PrestamoCreacionDTO dto) => 
            SendPostAsync<PrestamoDTO>("api/Prestamo/Creacion", dto);

        public Task<Result<PrestamoDTO>> ActualizarPrestamoAsync(int id, PrestamoDTO dto)
        {
            if (dto.IdPrestamo == 0) dto.IdPrestamo = id;
            return SendPutAsync<PrestamoDTO>("api/Prestamo/Edicion", dto);
        }
        #endregion

        #region PAGOS
        public Task<Result<PagoDTO>> RegistrarAbonoAsync(RegistrarAbonoDTO dto) => 
            SendPostAsync<PagoDTO>("api/Pago/RegistrarAbono", dto);

        public Task<Result<ResumenHistorialPagosDTO>> GetHistorialPagosAsync(int idPrestamo) => 
            SendGetAsync<ResumenHistorialPagosDTO>($"api/Pago/PorPrestamo/{idPrestamo}");
        #endregion

        #region USUARIOS
        public Task<Result<List<UsuarioDTO>>> GetUsuariosAsync() => 
            SendGetAsync<List<UsuarioDTO>>("api/Usuario");

        public Task<Result<UsuarioDTO>> CrearUsuarioAsync(UsuarioCreacionDTO dto) => 
            SendPostAsync<UsuarioDTO>("api/Usuario/Creacion", dto);

        public Task<Result<UsuarioDTO>> ActualizarUsuarioAsync(int id, UsuarioEdicionDTO dto)
        {
            if (dto.IdUsuario == 0) dto.IdUsuario = id;
            return SendPutAsync<UsuarioDTO>("api/Usuario/Edicion", dto);
        }

        public Task<Result<UsuarioDTO>> CambiarEstadoUsuarioAsync(int id) => 
            SendPutAsync<UsuarioDTO>($"api/Usuario/CambiarEstado/{id}", new { });
        #endregion

        #region DASHBOARD Y REPORTES
        public Task<Result<DashboardResumenDTO>> GetDashboardResumenAsync() => 
            SendGetAsync<DashboardResumenDTO>("api/Dashboard/resumen");

        public Task<Result<byte[]>> GenerarReporteExcelAsync(string titulo = "Reporte General de Cartera de Préstamos") => 
            SendGetAsync<byte[]>($"api/Dashboard/reporte-excel?titulo={Uri.EscapeDataString(titulo)}");
        #endregion
    }
}

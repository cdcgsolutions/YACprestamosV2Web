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

        private async Task<Result<T>> SendDeleteAsync<T>(string endpoint)
        {
            try
            {
                await SetTokenAsync();
                var response = await _httpClient.DeleteAsync(endpoint);
                return await ProcessResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                return new Result<T> { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        #endregion

        #region CLIENTES
        public Task<Result<List<ClienteDTO>>> GetClientesAsync(bool soloActivos = false) => 
            SendGetAsync<List<ClienteDTO>>($"api/Cliente?soloActivos={soloActivos}");

        public Task<Result<ResponseListDTO<ClienteDTO>>> GetClientesPaginadoAndBuscadorAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? q = null,
            int? idInstitucion = null,
            bool soloActivos = false)
        {
            var query = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}",
                $"soloActivos={soloActivos}"
            };
            if (!string.IsNullOrWhiteSpace(q)) query.Add($"q={Uri.EscapeDataString(q)}");
            if (idInstitucion.HasValue) query.Add($"idInstitucion={idInstitucion.Value}");
            return SendGetAsync<ResponseListDTO<ClienteDTO>>($"api/Cliente/PaginadoAndBuscador?{string.Join("&", query)}");
        }

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
            SendDeleteAsync<ClienteDTO>($"api/Cliente/{id}");
        #endregion

        #region INSTITUCIONES
        public Task<Result<List<InstitucionDTO>>> GetInstitucionesAsync(bool soloActivos = false) => 
            SendGetAsync<List<InstitucionDTO>>($"api/Institucion?soloActivos={soloActivos}");

        public Task<Result<ResponseListDTO<InstitucionDTO>>> GetInstitucionesPaginadoAndBuscadorAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? q = null,
            bool soloActivos = false)
        {
            var query = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}",
                $"soloActivos={soloActivos}"
            };
            if (!string.IsNullOrWhiteSpace(q)) query.Add($"q={Uri.EscapeDataString(q)}");
            return SendGetAsync<ResponseListDTO<InstitucionDTO>>($"api/Institucion/PaginadoAndBuscador?{string.Join("&", query)}");
        }

        public Task<Result<InstitucionDTO>> GetInstitucionByIdAsync(int id) => 
            SendGetAsync<InstitucionDTO>($"api/Institucion/{id}");

        public Task<Result<InstitucionDTO>> CrearInstitucionAsync(InstitucionDTO dto) => 
            SendPostAsync<InstitucionDTO>("api/Institucion/Creacion", dto);

        public Task<Result<InstitucionDTO>> ActualizarInstitucionAsync(int id, InstitucionDTO dto)
        {
            if (dto.IdInstitucion == 0) dto.IdInstitucion = id;
            return SendPutAsync<InstitucionDTO>("api/Institucion/Edicion", dto);
        }

        public Task<Result<InstitucionDTO>> CambiarEstadoInstitucionAsync(int id) => 
            SendDeleteAsync<InstitucionDTO>($"api/Institucion/{id}");
        #endregion

        #region PRESTAMOS
        public Task<Result<List<PrestamoDTO>>> GetPrestamosAsync(bool soloActivos = false) => 
            SendGetAsync<List<PrestamoDTO>>($"api/Prestamo?soloActivos={soloActivos}");

        public Task<Result<ResponseListDTO<PrestamoDTO>>> GetPrestamosPaginadoAndBuscadorAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? q = null,
            string? estado = null,
            int? idCliente = null)
        {
            var query = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}"
            };
            if (!string.IsNullOrWhiteSpace(q)) query.Add($"q={Uri.EscapeDataString(q)}");
            if (!string.IsNullOrWhiteSpace(estado)) query.Add($"estado={Uri.EscapeDataString(estado)}");
            if (idCliente.HasValue && idCliente.Value > 0) query.Add($"idCliente={idCliente.Value}");
            return SendGetAsync<ResponseListDTO<PrestamoDTO>>($"api/Prestamo/PaginadoAndBuscador?{string.Join("&", query)}");
        }

        public Task<Result<List<PrestamoDTO>>> GetPrestamosPorClienteAsync(int idCliente) => 
            SendGetAsync<List<PrestamoDTO>>($"api/Prestamo/PorCliente/{idCliente}");

        public Task<Result<PrestamoDTO>> GetPrestamoByIdAsync(int id) => 
            SendGetAsync<PrestamoDTO>($"api/Prestamo/{id}");

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

        public Task<Result<ResponseListDTO<UsuarioDTO>>> GetUsuariosPaginadoAndBuscadorAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? q = null,
            string? rol = null,
            bool soloActivos = false)
        {
            var query = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}",
                $"soloActivos={soloActivos}"
            };
            if (!string.IsNullOrWhiteSpace(q)) query.Add($"q={Uri.EscapeDataString(q)}");
            if (!string.IsNullOrWhiteSpace(rol)) query.Add($"rol={Uri.EscapeDataString(rol)}");
            return SendGetAsync<ResponseListDTO<UsuarioDTO>>($"api/Usuario/PaginadoAndBuscador?{string.Join("&", query)}");
        }

        public Task<Result<UsuarioDTO>> GetUsuarioByIdAsync(int id) => 
            SendGetAsync<UsuarioDTO>($"api/Usuario/{id}");

        public Task<Result<UsuarioDTO>> CrearUsuarioAsync(UsuarioCreacionDTO dto) => 
            SendPostAsync<UsuarioDTO>("api/Usuario/Creacion", dto);

        public Task<Result<UsuarioDTO>> ActualizarUsuarioAsync(int id, UsuarioEdicionDTO dto)
        {
            if (dto.IdUsuario == 0) dto.IdUsuario = id;
            return SendPutAsync<UsuarioDTO>("api/Usuario/Edicion", dto);
        }

        public Task<Result<UsuarioDTO>> CambiarEstadoUsuarioAsync(int id) => 
            SendDeleteAsync<UsuarioDTO>($"api/Usuario/{id}");
        #endregion

        #region DASHBOARD Y REPORTES
        public Task<Result<DashboardResumenDTO>> GetDashboardResumenAsync(
            int? idInstitucion = null,
            string? modalidad = null,
            string? estado = null,
            int? mes = null,
            int? ano = null,
            DateTime? fechaInicio = null, 
            DateTime? fechaFin = null)
        {
            var query = new List<string>();
            if (idInstitucion.HasValue) query.Add($"idInstitucion={idInstitucion.Value}");
            if (!string.IsNullOrWhiteSpace(modalidad) && modalidad != "Todos") query.Add($"modalidad={Uri.EscapeDataString(modalidad)}");
            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos") query.Add($"estado={Uri.EscapeDataString(estado)}");
            if (mes.HasValue && mes.Value > 0) query.Add($"mes={mes.Value}");
            if (ano.HasValue && ano.Value > 0) query.Add($"ano={ano.Value}");
            if (fechaInicio.HasValue) query.Add($"fechaInicio={fechaInicio.Value:yyyy-MM-dd}");
            if (fechaFin.HasValue) query.Add($"fechaFin={fechaFin.Value:yyyy-MM-dd}");
            var qs = query.Count > 0 ? "?" + string.Join("&", query) : "";
            return SendGetAsync<DashboardResumenDTO>($"api/Dashboard/resumen{qs}");
        }

        public Task<Result<byte[]>> GenerarReporteExcelAsync(
            int? idInstitucion = null,
            string? modalidad = null,
            string? estado = null,
            int? mes = null,
            int? ano = null,
            DateTime? fechaInicio = null, 
            DateTime? fechaFin = null, 
            string titulo = "Reporte Ejecutivo de Cartera de Préstamos")
        {
            var query = new List<string> { $"titulo={Uri.EscapeDataString(titulo)}" };
            if (idInstitucion.HasValue) query.Add($"idInstitucion={idInstitucion.Value}");
            if (!string.IsNullOrWhiteSpace(modalidad) && modalidad != "Todos") query.Add($"modalidad={Uri.EscapeDataString(modalidad)}");
            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos") query.Add($"estado={Uri.EscapeDataString(estado)}");
            if (mes.HasValue && mes.Value > 0) query.Add($"mes={mes.Value}");
            if (ano.HasValue && ano.Value > 0) query.Add($"ano={ano.Value}");
            if (fechaInicio.HasValue) query.Add($"fechaInicio={fechaInicio.Value:yyyy-MM-dd}");
            if (fechaFin.HasValue) query.Add($"fechaFin={fechaFin.Value:yyyy-MM-dd}");
            return SendGetAsync<byte[]>($"api/Dashboard/reporte-excel?{string.Join("&", query)}");
        }
        #endregion
    }
}

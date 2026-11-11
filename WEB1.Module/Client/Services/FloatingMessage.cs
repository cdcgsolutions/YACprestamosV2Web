using Microsoft.JSInterop;

namespace WEB1.Module.Client.Services
{
    /// <summary>
    /// Servicio para mostrar notificaciones flotantes temporales (Toastr).
    /// </summary>
    public class FloatingMessage
    {
        private readonly IJSRuntime _jsRuntime;

        public FloatingMessage(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task Success(string message, string title = "Éxito")
            => await ShowAsync("success", message, title);

        public async Task Error(string message, string title = "Error")
            => await ShowAsync("error", message, title);

        public async Task ShowErrorMessageWithTitle(string message, string title = "Error")
            => await ShowAsync("error", message, title);

        public async Task MostrarMensajeErrorWithTitle(string message, string title = "Error")
            => await ShowAsync("error", message, title);

        public async Task ShowSuccessMessageWithTitle(string message, string title = "Éxito")
            => await ShowAsync("success", message, title);

        public async Task MostrarMensajeSuccessWithTitle(string message, string title = "Éxito")
            => await ShowAsync("success", message, title);

        public async Task Warning(string message, string title = "Advertencia")
            => await ShowAsync("warning", message, title);

        public async Task Info(string message, string title = "Información")
            => await ShowAsync("info", message, title);

        public async Task ShowAsync(string type, string message, string title = "")
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("AlertsToastr", type.ToLower(), message, title);
            }
            catch
            {
                // Ignorar excepciones durante prerender
            }
        }

        public async Task ClearAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("clearToastr");
            }
            catch
            {
                // Ignorar excepciones durante prerender
            }
        }
    }
}

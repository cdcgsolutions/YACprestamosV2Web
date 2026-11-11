using Microsoft.JSInterop;

namespace WEB1.Module.Client.Services
{
    /// <summary>
    /// Servicio para mostrar mensajes modales centrados y confirmaciones interactivas (SweetAlert2).
    /// </summary>
    public class FocusedMessage
    {
        private readonly IJSRuntime _jsRuntime;

        public FocusedMessage(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task Alert(string title, string text = "", string icon = "info", string theme = "auto")
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("alerta", title, text, icon, theme);
            }
            catch
            {
                // Ignorar excepciones durante prerender
            }
        }

        public async Task Success(string title, string text = "", string theme = "auto")
            => await Alert(title, text, "success", theme);

        public async Task Error(string title, string text = "", string theme = "auto")
            => await Alert(title, text, "error", theme);

        public async Task Warning(string title, string text = "", string theme = "auto")
            => await Alert(title, text, "warning", theme);

        public async Task Info(string title, string text = "", string theme = "auto")
            => await Alert(title, text, "info", theme);

        /// <summary>
        /// Muestra un diálogo modal de confirmación con botones Sí/No.
        /// </summary>
        public async Task<bool> Confirm(string title, string text, string icon = "question", string theme = "auto")
        {
            try
            {
                return await _jsRuntime.InvokeAsync<bool>("confirmar", title, text, icon, theme);
            }
            catch
            {
                return false;
            }
        }
    }
}

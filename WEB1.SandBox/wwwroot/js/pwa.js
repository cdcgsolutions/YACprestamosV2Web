// ==========================================================================
// Controlador PWA para Blazor (Registro de Service Worker e Instalación)
// ==========================================================================

window.pwaDeferredPrompt = null;

// 1. Registro del Service Worker
if ("serviceWorker" in navigator) {
    window.addEventListener("load", () => {
        navigator.serviceWorker.register("/sw.js")
            .then((registro) => {
                console.log("[PWA] Service Worker registrado con éxito en alcance:", registro.scope);
            })
            .catch((error) => {
                console.warn("[PWA] Error al registrar Service Worker:", error);
            });
    });
}

// 2. Captura del evento de instalación nativo
window.addEventListener("beforeinstallprompt", (e) => {
    // Prevenir el banner automático intrusivo para controlarlo
    e.preventDefault();
    window.pwaDeferredPrompt = e;
    console.log("[PWA] Evento beforeinstallprompt capturado. La aplicación es instalable.");
    window.dispatchEvent(new CustomEvent("pwa-installable", { detail: { installable: true } }));
});

// 3. Confirmación de instalación completada
window.addEventListener("appinstalled", () => {
    window.pwaDeferredPrompt = null;
    console.log("[PWA] ¡YAC-Préstamos fue instalada exitosamente en el dispositivo!");
    if (typeof toastr !== "undefined") {
        toastr.success("¡Aplicación instalada exitosamente en tu dispositivo!", "PWA Lista");
    }
    window.dispatchEvent(new CustomEvent("pwa-installable", { detail: { installable: false } }));
});

// 4. API global accesible desde JavaScript o Blazor C# via IJSRuntime
window.pwaHelper = {
    isInstallable: function () {
        return !!window.pwaDeferredPrompt;
    },
    isStandalone: function () {
        return window.matchMedia("(display-mode: standalone)").matches || window.navigator.standalone === true;
    },
    promptInstall: async function () {
        if (!window.pwaDeferredPrompt) {
            console.warn("[PWA] No hay evento de instalación pendiente.");
            return false;
        }
        window.pwaDeferredPrompt.prompt();
        const choice = await window.pwaDeferredPrompt.userChoice;
        window.pwaDeferredPrompt = null;
        return choice.outcome === "accepted";
    }
};

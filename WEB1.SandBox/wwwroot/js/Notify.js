// Comprobar si el navegador soporta la API de Notificaciones
if ("Notification" in window) {

    // Función para solicitar permiso al usuario
    function askNotificationPermission() {
        if (Notification.permission === "default") {
            Notification.requestPermission().then(permission => {
                if (permission === "granted") {
                    console.log("Permiso para notificaciones concedido");
                } else {
                    console.log("Permiso para notificaciones denegado");
                }
            });
        }
    }

    // Función para mostrar una notificación
    function showNotification(notificacionx) {
        console.log(notificacionx);
        if (Notification.permission === "granted") {
            const notification =  new Notification("¡Tienes una nueva notificación de " + notificacionx.title + "!", {
                body: notificacionx.subTitle + " - " + notificacionx.message,
                icon: "icono.png", // Puedes poner una ruta a un icono
                tag: "notificación-nueva" // Etiqueta para identificarla
            });

            notification.onclick = function () {
                // Redirigir a la URL deseada
                window.location.href = "https://www.ejemplo.com"; // Cambia la URL a la página de destino
            };
        } else {
            console.log("Permiso para notificaciones no concedido.");
        }
    }

    // Cuando se hace clic en el botón, solicitar permiso y mostrar una notificación
    function ShowAlerts(notificacion) {
        askNotificationPermission(); // Solicitar permisos
        setTimeout(showNotification(notificacion), 2000); // Mostrar notificación después de 2 segundos
    }


} else {
    console.log("Este navegador no soporta notificaciones.");
}
const NombreCache = "yac-prestamos-blazor-v2";

// Recursos estáticos prioritarios para el cascarón (App Shell)
const RecursosCache = [
  "/",
  "/manifest.json",
  "/iconos/icon.svg",
  "/iconos/icon.png",
  "/brand/logoyac.png",
  "/css/app.css",
  "/css/variables.css",
  "/favicon.ico"
];

// Instalación: Pre-cachear recursos críticos y activar inmediatamente
self.addEventListener("install", (evento) => {
  self.skipWaiting();
  evento.waitUntil(
    caches.open(NombreCache).then((cache) => {
      return cache.addAll(RecursosCache).catch((err) => {
        console.warn("[PWA SW] Advertencia al pre-cachear recursos:", err);
      });
    })
  );
});

// Activación: Limpieza de cachés antiguas y toma de control inmediata
self.addEventListener("activate", (evento) => {
  evento.waitUntil(
    caches.keys().then((claves) => {
      return Promise.all(
        claves.map((clave) => {
          if (clave !== NombreCache) {
            console.log("[PWA SW] Eliminando caché obsoleta:", clave);
            return caches.delete(clave);
          }
        })
      );
    }).then(() => self.clients.claim())
  );
});

// Intercepción de solicitudes
self.addEventListener("fetch", (evento) => {
  // Ignorar métodos que no sean GET
  if (evento.request.method !== "GET") return;

  const url = new URL(evento.request.url);

  // 1. REGLA DE ORO PARA BLAZOR: Nunca interceptar SignalR / WebSockets
  if (url.pathname.includes("/_blazor") || url.protocol.startsWith("ws")) {
    return;
  }

  // 2. Nunca interceptar llamadas a APIs de datos dinámicos
  if (url.pathname.startsWith("/api/") || url.pathname.includes("/swagger") || url.hostname.includes("runasp.net") && url.pathname.startsWith("/api")) {
    return;
  }

  // 3. Estrategia para archivos estáticos (CSS, JS, Fuentes, Imágenes, Iconos)
  const esRecursoEstatico =
    url.pathname.endsWith(".css") ||
    url.pathname.endsWith(".js") ||
    url.pathname.endsWith(".woff2") ||
    url.pathname.endsWith(".woff") ||
    url.pathname.endsWith(".ttf") ||
    url.pathname.endsWith(".png") ||
    url.pathname.endsWith(".jpg") ||
    url.pathname.endsWith(".svg") ||
    url.pathname.endsWith(".ico") ||
    url.hostname.includes("cdn.jsdelivr.net") ||
    url.hostname.includes("cdnjs.cloudflare.com");

  if (esRecursoEstatico) {
    // Cache First con fallback a red y actualización dinámica
    evento.respondWith(
      caches.match(evento.request).then((respuestaCache) => {
        if (respuestaCache) {
          // Si está en caché, devolverlo inmediatamente y actualizar en segundo plano
          fetch(evento.request).then((respuestaRed) => {
            if (respuestaRed && respuestaRed.status === 200) {
              const clon = respuestaRed.clone();
              caches.open(NombreCache).then((c) => c.put(evento.request, clon));
            }
          }).catch(() => { });
          return respuestaCache;
        }

        // Si no está en caché, pedir a la red y guardar copia
        return fetch(evento.request).then((respuestaRed) => {
          if (respuestaRed && respuestaRed.status === 200) {
            const clon = respuestaRed.clone();
            caches.open(NombreCache).then((c) => c.put(evento.request, clon));
          }
          return respuestaRed;
        });
      })
    );
    return;
  }

  // 4. Estrategia para navegación de páginas (HTML): Network First con fallback a caché
  if (evento.request.mode === "navigate") {
    evento.respondWith(
      fetch(evento.request)
        .then((respuestaRed) => {
          if (respuestaRed && respuestaRed.status === 200) {
            const clon = respuestaRed.clone();
            caches.open(NombreCache).then((c) => c.put(evento.request, clon));
          }
          return respuestaRed;
        })
        .catch(() => {
          return caches.match(evento.request).then((respuestaCache) => {
            return respuestaCache || caches.match("/");
          });
        })
    );
    return;
  }
});

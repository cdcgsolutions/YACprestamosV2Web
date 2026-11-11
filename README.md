# 🌐 WEB_FRONT - Manual de Uso y Arquitectura Blazor .NET 10

Plantilla Frontend desacoplada, modular y de nivel empresarial desarrollada en **Blazor (.NET 10)**. Incorpora **MudBlazor (Material Design)**, **Bootstrap 5**, autenticación **JWT autónoma**, sistema de módulos dinámicos (*Plantilla vs BBDD*), catálogo de utilidades desacopladas (`GenericDragAndDrop`, `FloatingMessage`, `FocusedMessage`, `AppModal`, `ListadoGenerico`) y selector de temas Claro/Oscuro.

---

## 📑 Tabla de Contenidos
1. [🏗️ Estructura del Proyecto](#-estructura-del-proyecto)
2. [⚙️ Configuración (`appsettings.json`)](#-configuración-appsettingsjson)
3. [🔄 Sistema de Módulos: Modo Plantilla vs Modo API/BBDD](#-sistema-de-módulos-modo-plantilla-vs-modo-apibbdd)
4. [🧰 Catálogo de Componentes y Utilidades](#-catálogo-de-componentes-y-utilidades)
   - [GenericDragAndDrop (Carga de Archivos)](#1-genericdraganddrop)
   - [FloatingMessage (Notificaciones Toastr)](#2-floatingmessage)
   - [FocusedMessage (Alertas y Confirmaciones SweetAlert2)](#3-focusedmessage)
   - [AppModal (Ventanas Modales Bootstrap 5)](#4-appmodal)
   - [ListadoGenerico (Gestión de Estados de Listas)](#5-listadogenerico)
5. [🔐 Autenticación y Seguridad](#-autenticación-y-seguridad)
6. [🎨 Temas y Paleta de Comandos (`Ctrl + K`)](#-temas-y-paleta-de-comandos-ctrl--k)
7. [🚀 Ejecución y Despliegue](#-ejecución-y-despliegue)

---

## 🏗️ Estructura del Proyecto

```
WEB_FRONT/
├── WEB1.SandBox/                     # Proyecto Host / Web App principal (Blazor Web App .NET 10)
│   ├── Authentication/               # CustomAuthStateProvider y JwtParser
│   ├── Components/
│   │   ├── Layout/                   # MainLayout, NavMenu, CommandPalette (Ctrl+K)
│   │   └── Pages/                    # Login, Home (Grid Módulos), ModuleView, Rutas 404/403
│   ├── DTOs/                         # AuthDTOs, NavigationDTOs, ModuleDTO, InterfaceDTO
│   ├── Services/                     # ModuleService, AuthService, LayoutService
│   └── appsettings.json              # Configuración de URLs y variables de entorno
│
└── WEB1.Module/                      # Razor Class Library (RCL) - Librería Reutilizable
    ├── Client/
    │   └── Services/                 # Componentes y utilidades nativas desacopladas
    │       ├── GenericDragAndDrop.razor   # Drag & Drop con streaming y previews
    │       ├── AppModal.razor             # Modales Bootstrap 5 tipados
    │       ├── ListadoGenerico.razor      # Renderizado de listas genéricas (@typeparam)
    │       ├── ProgressBar.razor          # Barra de carga de archivos
    │       ├── FloatingMessage.cs         # Wrapper nativo Toastr
    │       ├── FocusedMessage.cs          # Wrapper nativo SweetAlert2
    │       └── DragDropModels.cs          # Modelos y enums bilingües
    └── Extensions/                   # Métodos de extensión para inyección de dependencias
```

---

## ⚙️ Configuración (`appsettings.json`)

Edita el archivo `WEB1.SandBox/appsettings.json`:

```json
{
  "Apis": {
    "Default": "https://localhost:7082/",
    "ModulesEndpoint": "api/Modules"
  },
  "app": {
    "Name": "WEBAPPN10",
    "Mode": "DEV",
    "Version": "1.0.0",
    "Plantilla": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

* **`Apis:Default`**: URL base del Backend (`WEB_API_N10` en local o tu URL en Render).
* **`Apis:ModulesEndpoint`**: Ruta del endpoint que entrega los módulos autorizados (por defecto `api/Modules`).
* **`app:Plantilla`**: 
  - `true`: Modo Desarrollo / Maquetación (usa tarjetas y módulos mock hardcodeados en memoria).
  - `false`: Modo Producción / API (consulta a tu backend real con el JWT Token del usuario).

---

## 🔄 Sistema de Módulos: Modo Plantilla vs Modo API/BBDD

El servicio `ModuleService` gestiona dinámicamente qué pantallas y módulos ve el usuario según el flag `"Plantilla"`:

### 1. Modo Plantilla (`"Plantilla": true`)
Ideal para **desarrollo ágil y diseño de nuevas interfaces**:
* Puedes crear nuevas tarjetas en `ModuleService.LoadMockModules()`.
* No requiere que el backend ni la base de datos Firestore estén levantados.
* Permite probar estilos, iconos y navegación de inmediato.

### 2. Modo Real API (`"Plantilla": false`)
Ideal para **pruebas de integración y producción**:
* `ModuleService.GetModulesAsync()` extrae automáticamente el **Token JWT** del usuario autenticado.
* Hace una petición `GET api/Modules` al Backend.
* Tu Backend filtra según el Rol del usuario en Firestore y retorna únicamente los módulos y vistas autorizados.
* Si el backend no está disponible o el usuario no tiene acceso, el sistema muestra de forma elegante el mensaje de estado vacío sin romper la navegación.

---

## 🧰 Catálogo de Componentes y Utilidades

Todos los componentes residen en el namespace `WEB1.Module.Client.Services`.

### 1. `GenericDragAndDrop`
Componente avanzado para carga de archivos con soporte para arrastrar y soltar, subida por chunks (streaming), barra de progreso y previsualización de imágenes, PDFs y documentos.

```razor
@using WEB1.Module.Client.Services

<GenericDragAndDrop 
    Cantidad="archivo.multiple" 
    Tipo="Accept.Imagenes" 
    MaxSize="10" 
    MinHeight="250px" 
    TaskFileProcess="ProcesarArchivosAsync" />

@code {
    private async Task ProcesarArchivosAsync(TaskProcess taskProcess)
    {
        // Obtiene la lista de archivos subidos
        var archivos = taskProcess.Files;
        foreach (var file in archivos)
        {
            Console.WriteLine($"Archivo: {file.Name}, Tamaño: {file.Size} bytes");
        }
    }
}
```

---

### 2. `FloatingMessage`
Notificaciones tipo *Toastr* flotantes en pantalla con auto-cierre y animaciones:

```csharp
@inject FloatingMessage Toast

// Mensajes estándar
await Toast.Success("Operación completada con éxito.");
await Toast.Error("Ocurrió un error al guardar los datos.");
await Toast.Warning("Atención: El registro está por vencer.");
await Toast.Info("Información: Hay nuevas actualizaciones.");

// Mensajes con título personalizado
await Toast.MostrarMensajeSuccessWithTitle("¡Guardado!", "El cliente fue registrado.");
await Toast.MostrarMensajeErrorWithTitle("Error Crítico", "No se pudo conectar a la API.");
```

---

### 3. `FocusedMessage`
Modales interactivos y diálogos de confirmación con *SweetAlert2*:

```csharp
@inject FocusedMessage Alert

// Alerta informativa
await Alert.ShowSuccess("Datos guardados correctamente");
await Alert.ShowError("No se pudo procesar la solicitud");

// Diálogo de Confirmación (Retorna bool)
bool confirmado = await Alert.Confirm(
    title: "¿Está seguro de eliminar este registro?",
    text: "Esta acción no se puede deshacer.",
    confirmButtonText: "Sí, eliminar",
    cancelButtonText: "Cancelar"
);

if (confirmado)
{
    // Ejecutar lógica de borrado
}
```

---

### 4. `AppModal`
Modal nativo basado en Bootstrap 5 que soporta tamaños dinámicos, títulos, encabezados con icono y botones de acción:

```razor
@using WEB1.Module.Client.Services

<button class="btn btn-primary" @onclick="() => modalRef.OpenModal()">
    Abrir Modal
</button>

<AppModal @ref="modalRef" 
          Title="Editar Usuario" 
          TitleIcon="bi bi-person-gear" 
          ModalSize="SizeModal.Large" 
          OnSubmit="GuardarCambios">
    <Body>
        <div class="mb-3">
            <label class="form-label">Nombre Completo</label>
            <input type="text" class="form-control" @bind="usuario.Nombre" />
        </div>
    </Body>
    <Footer>
        <button type="button" class="btn btn-secondary" @onclick="() => modalRef.CloseModal()">Cerrar</button>
        <button type="button" class="btn btn-primary" @onclick="GuardarCambios">Guardar</button>
    </Footer>
</AppModal>

@code {
    private AppModal modalRef;
    private void GuardarCambios() { modalRef.CloseModal(); }
}
```

---

### 5. `ListadoGenerico`
Componente que abstrae el manejo de listas en 3 estados: Cargando (Spinner), Sin Registros (Alert informativo) y Con Registros.

```razor
@using WEB1.Module.Client.Services

<ListadoGenerico Listado="_usuarios">
    <Cargando>
        <div class="text-center py-4">
            <div class="spinner-border text-primary" role="status"></div>
            <p class="mt-2 text-muted">Cargando usuarios...</p>
        </div>
    </Cargando>
    <NoHayRegistros>
        <div class="alert alert-info">No existen usuarios registrados actualmente.</div>
    </NoHayRegistros>
    <HayRegistros Context="usuario">
        <div class="card p-3 mb-2 shadow-xs">
            <h6>@usuario.Nombre</h6>
            <small class="text-muted">@usuario.Email</small>
        </div>
    </HayRegistros>
</ListadoGenerico>
```

---

## 🔐 Autenticación y Seguridad

* **Login:** Integrado contra `POST api/Auth/login`. Al autenticarse, almacena el JWT en `localStorage` y actualiza `CustomAuthStateProvider`.
* **Rutas Protegidas:** Las vistas usan la directiva `@attribute [Authorize]`. Si un usuario anónimo intenta ingresar, es redirigido automáticamente a `/login`.
* **Páginas de Error de Acceso:**
  - **404 (No Encontrado):** Pantalla seria con tarjeta central, borde de alerta rojo y botón de retorno al inicio.
  - **403 (No Autorizado):** Aviso formal cuando el rol del usuario no tiene permisos suficientes.

---

## 🎨 Temas y Paleta de Comandos (`Ctrl + K`)

1. **Selector de Temas:**
   - Permite cambiar entre **Modo Claro**, **Modo Oscuro** y **Automático (Sistema)**.
   - Sincronizado tanto con las clases de Bootstrap 5 como con los tokens de MudBlazor.
2. **Command Palette (`Ctrl + K`):**
   - Buscador universal estilo Spotlight accesible desde cualquier parte de la aplicación.
   - Permite filtrar interfaces y módulos instantáneamente con navegación por teclado (Flechas ↑ / ↓ y Enter).

---

## 🚀 Ejecución y Despliegue

### Ejecución Local

1. Abrir la solución `WEB1.sln` en Visual Studio o VS Code.
2. Asegurar que `WEB1.SandBox` sea el proyecto de inicio.
3. Ejecutar por terminal:
   ```bash
   dotnet run --project WEB1.SandBox/WEB1.SandBox.csproj
   ```
4. Abrir en el navegador: `https://localhost:7062/` (o el puerto configurado en `launchSettings.json`).

### Despliegue en la Nube (Render / Docker)

Para publicar en **Render**:
1. Crear un **Web Service** conectado a tu repositorio `WEB_FRONT`.
2. Especificar Dockerfile o Runtime .NET 10.
3. Configurar las Variables de Entorno en el Dashboard de Render:
   - `Apis__Default` = `https://tu-api-backend.onrender.com/`
   - `app__Plantilla` = `false`
   - `app__Mode` = `PRO`

---

## 📄 Licencia
Plantilla base de desarrollo modular e independiente. Desarrollada para máxima extensibilidad y escalabilidad.

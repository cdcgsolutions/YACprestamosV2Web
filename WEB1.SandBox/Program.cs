using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using WEB1.SandBox.Authentication;
using WEB1.SandBox.Configuration;
using WEB1.SandBox.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// 1. Componentes Razor Interactivos (.NET 10)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// 2. Servicios de MudBlazor (UI Material Design)
builder.Services.AddMudServices();

// 3. Autenticación y Autorización con Redirección a /login
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "BlazorAuthScheme";
    options.DefaultChallengeScheme = "BlazorAuthScheme";
})
.AddCookie("BlazorAuthScheme", options =>
{
    options.LoginPath = "/login";
});

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ModuleService>();

// 4. Cliente HTTP para consumir tu API Backend desacoplada (WEB_API_N10)
builder.Services.AddHttpClient("BackendApi", client =>
{
    var baseUrl = configuration["Apis:Default"] ?? "https://localhost:7082/";
    client.BaseAddress = new Uri(baseUrl);
});

// 5. Configuración de Inyección de Dependencias
builder.Services.AddConfRN();
builder.Services.AddServiceModules(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();

// Componentes y Ensamblados de Módulos
app.AddComponentsModule();

app.Run();

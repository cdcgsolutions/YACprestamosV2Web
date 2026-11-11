using WEB1.SandBox.Components;

namespace WEB1.SandBox.Configuration
{
    public static class confModule
    {
        public static WebApplication AddComponentsModule(this WebApplication app)
        {
            app.MapRazorComponents<App>()
               .AddInteractiveServerRenderMode()
               .AddAdditionalAssemblies(
               [
                   typeof(WEB1.Module._Imports).Assembly,
               ]);

            return app;
        }

        public static IServiceCollection AddServiceModules(this IServiceCollection services, IConfiguration configuration)
        {
            WEB1.Module.CLIENT.ServiceCollectionExtensions.AddServiceTemplate(services, configuration);
            return services;
        }
    }
}

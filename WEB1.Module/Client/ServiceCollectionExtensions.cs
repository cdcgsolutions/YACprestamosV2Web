using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WEB1.Module.CLIENT.Services;
using WEB1.Module.Client.Services;

namespace WEB1.Module.CLIENT
{
    public static class ServiceCollectionExtensions
    {
        private static IServiceCollection AddTemplateAPI(IServiceCollection services)
        {
            var ruleTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.Contains("RN") && !t.IsAbstract);

            foreach (var ruleType in ruleTypes)
            {
                services.AddScoped(ruleType);
            }
            return services;
        }

        public static IServiceCollection AddServiceTemplate(IServiceCollection services, IConfiguration configuration)
        {
            AddTemplateAPI(services);
            services.AddScoped<IUserPreferenceService, UserPreferenceService>();
            services.AddScoped<LayoutService>();
            services.AddScoped<FloatingMessage>();
            services.AddScoped<FocusedMessage>();

            return services;
        }
    }
}

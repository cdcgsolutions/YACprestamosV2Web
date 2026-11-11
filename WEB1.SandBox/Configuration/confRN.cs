using System.Reflection;

namespace WEB1.SandBox.Configuration
{
    public static class confRN
    {
        public static IServiceCollection AddConfRN(this IServiceCollection services)
        {
            var ruleTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.Contains("Injections") && !t.IsAbstract);

            foreach (var ruleType in ruleTypes)
            {
                services.AddScoped(ruleType);
            }

            return services;
        }
    }
}

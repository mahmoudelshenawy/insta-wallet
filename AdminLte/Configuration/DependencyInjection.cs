using AdminLte.Areas.Repositories;
using AdminLte.Models;
using AdminLte.Providers;
using AdminLte.Repositories;
using AdminLte.Services;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace AdminLte.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection InstallServices(this IServiceCollection services, IConfiguration configuration,
            params Assembly[] assemblies)
        {
            IEnumerable<IServiceInstaller> serviceInstallers = assemblies.SelectMany(a => a.DefinedTypes).Where(IsAssignableToType<IServiceInstaller>)
                  .Select(Activator.CreateInstance).Cast<IServiceInstaller>();

            foreach (IServiceInstaller serviceInstaller in serviceInstallers)
            {
                serviceInstaller.Install(services, configuration);
            }

            return services;
        }
        static bool IsAssignableToType<T>(TypeInfo typeInfo) => typeInfo.IsAssignableFrom(typeof(T))
        && !typeInfo.IsInterface && !typeInfo.IsAbstract;


    }

    
}

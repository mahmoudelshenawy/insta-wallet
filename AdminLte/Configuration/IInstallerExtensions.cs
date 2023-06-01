using System.Reflection;

namespace AdminLte.Configuration
{
    public static class IInstallerExtensions
    {
        public static void InstallServicesFromAssymblyContaining<TKarker>(this IServiceCollection services, IConfiguration configuration)
        {
            InstallServicesFromAssymbliesContaining(services, configuration, typeof(TKarker));
        }

        public static void InstallServicesFromAssymbliesContaining(this IServiceCollection services, IConfiguration configuration, params Type[] assymplyMarkers)
        {

            var assymples = assymplyMarkers.Select(x => x.Assembly).ToArray();
            InstallServicesFromAssymblies(services, configuration, assymples);

        }

        public static void InstallServicesFromAssymblies(this IServiceCollection services, IConfiguration configuration, Assembly[] assymplies)
        {

            foreach (var assymbly in assymplies)
            {
                var installerTypes = assymbly.DefinedTypes
                    .Where(x => typeof(IServiceInstaller).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface);

                var installers = installerTypes.Select(Activator.CreateInstance).Cast<IServiceInstaller>();

                foreach (var install in installers.OrderBy(x => x.order))
                {
                    install.Install(services, configuration);
                }
            }

        }
    }
}

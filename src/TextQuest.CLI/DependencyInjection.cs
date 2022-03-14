using Microsoft.Extensions.DependencyInjection;
using TextQuest.CLI.Interfaces;

namespace TextQuest.CLI
{
    public static class DependencyInjection
    {
        public static void AddCLI(this IServiceCollection services)
        {
            services.AddScoped<IApplication, ApplicationFacade>();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using TextQuest.Application.Interfaces;
using TextQuest.Infrastructure.Services;

namespace TextQuest.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<INameGenerator, NameGenerator>();
            services.AddSingleton<IRandom, RandomAdapter>();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using TextQuest.Application.Interfaces;
using TextQuest.Application.Services;

namespace TextQuest.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IWorldProvider, WorldProvider>();
            services.AddScoped<IPlayerController, PlayerController>();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using TextQuest.Application;
using TextQuest.CLI;
using TextQuest.CLI.Interfaces;
using TextQuest.Infrastructure;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        using var serviceProvider = GetServiceProvider();

        var app = serviceProvider.GetRequiredService<IApplication>();
        app.Run();
    }

    private static ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddApplication();
        services.AddInfrastructure();
        services.AddCLI();
        services.AddSingleton<IApplication, ApplicationFacade>();

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider;
    }
}
using Microsoft.Extensions.DependencyInjection;
using TextQuest.Application;
using TextQuest.CLI;
using TextQuest.CLI.Interfaces;
using TextQuest.Infrastructure;

internal static class Program
{
    private static void Main()
    {
        Console.InputEncoding =
            Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.Title = "TextQuest";
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;

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
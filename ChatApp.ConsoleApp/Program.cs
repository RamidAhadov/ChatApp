using Autofac;
using ChatApp.Business.Abstraction;
using ChatApp.Business.DependencyResolvers.Autofac;

namespace ChatApp.ConsoleApp;

internal class Program
{
    private static IServerConnectionService _serverConnectionService;
    static async Task Main(string[] args)
    {
        
        var builder = new ContainerBuilder(); 
    
        builder.RegisterModule(new AutofacBusinessModule());
    
        var container = builder.Build();

        await using var scope = container.BeginLifetimeScope();

        _serverConnectionService = scope.Resolve<IServerConnectionService>();

        _serverConnectionService.EstablishConnection();

        await Task.Run(ReceiveMessages);
        
        // ReSharper disable once FunctionNeverReturns
    }
    

    static async Task ReceiveMessages()
    {
        while (true)
        {
            Console.WriteLine(await _serverConnectionService.GetMessagesAsync());
        }
    }
}
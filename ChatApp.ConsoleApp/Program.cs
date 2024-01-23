using Autofac;
using ChatApp.Business.Abstraction;
using ChatApp.Business.DependencyResolvers.Autofac;
using ChatApp.Configuration.Abstraction;
using ChatApp.Core.Utilities.Protocols;

namespace ChatApp.ConsoleApp;

internal class Program
{
    private static IConnectionService _connectionService;
    private static ICheckPortService _portService;
    private static IConnectionParameter _connectionParameter;
    static async Task Main(string[] args)
    {
        var builder = new ContainerBuilder(); 
    
        builder.RegisterModule(new AutofacBusinessModule());
    
        var container = builder.Build();

        await using var scope = container.BeginLifetimeScope();

        _portService = scope.Resolve<ICheckPortService>();

        _connectionParameter = scope.Resolve<IConnectionParameter>();

        var condition = _portService.IsListening(_connectionParameter.Port);

        if (condition)
        {
            _connectionService = scope.ResolveNamed<IConnectionService>("Server");
            
            Console.WriteLine($"Connection will be establish as server. Host: {InternetProtocol.GetCurrentIPv4Address()}:{_connectionParameter.Port}");
        }
        else
        {
            _connectionService = scope.ResolveNamed<IConnectionService>("Client");
            
            Console.WriteLine("Connection will be establish as client.\nPlease enter host ip and port.");
        }
        
        _connectionService.EstablishConnection();
        Console.WriteLine("Connection successfully established.");
        
        Task receive = Task.Run(ReceiveMessagesAsync);
        
        Task send =  Task.Run(SendMessagesAsync);

        await Task.WhenAll(receive, send);
    }
    

    static async Task ReceiveMessagesAsync()
    {
        Console.WriteLine("Receive "+Thread.CurrentThread.ManagedThreadId);
        while (true)
        {
            Console.WriteLine(await _connectionService.GetMessagesAsync());
        }
    }

    static async Task SendMessagesAsync()
    
    {
        Console.WriteLine("Send "+Thread.CurrentThread.ManagedThreadId);
        while (true)
        {
            var message = Console.ReadLine();
            if (message != null)
                await _connectionService.SendMessageAsync(message);
        }
    }
}
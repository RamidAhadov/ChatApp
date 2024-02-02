using System.Net.Sockets;
using Autofac;
using ChatApp.Business.Abstraction;
using ChatApp.Business.DependencyResolvers.Autofac;
using ChatApp.Configuration.Abstraction;
using ChatApp.ConsoleApp.MagicString;
using ChatApp.Core.Utilities.Protocols;

namespace ChatApp.ConsoleApp;

internal class Program
{
    private static IConnectionService _connectionService;
    private static ICheckPortService _portService;
    private static IConnectionParameter _connectionParameter;
    private static IServerConnectionService _serverConnectionService;

    private static async Task Main(string[] args)
    {
        var scope = Scope();

        _portService = scope.Resolve<ICheckPortService>();

        _connectionParameter = scope.Resolve<IConnectionParameter>();

        Console.WriteLine("Please choose connection type. \nLocal or Global");
        var userResponse = Console.ReadLine()!.ToLower();
        while (!ResponseControl(userResponse))
        {
            Console.WriteLine(ConstantMessages.WrongUserResponse);
            userResponse = Console.ReadLine()!.ToLower();
        }

        if (userResponse == "global")
        {
            Console.WriteLine("Connection will be establish on local.");
            Console.WriteLine("Please choose user type. \nServer or Client");
            
            var globalUserResponse = Console.ReadLine()!.ToLower();
            while (!GlobalResponseControl(globalUserResponse))
            {
                Console.WriteLine(ConstantMessages.WrongUserResponse);
                globalUserResponse = Console.ReadLine()!.ToLower();
            }

            if (globalUserResponse == "server")
            {
                await EstablishTcpConnection(scope,true);
            }
            else
            {
                await EstablishTcpConnection(scope,false);
            }
            
        }
        else if(userResponse == "local")
        {
            Console.WriteLine("Connection will be establish on local.");
            var condition = _portService.IsListening(_connectionParameter.Port);
            await EstablishTcpConnection(scope,condition);
        }
    }

    
    private static async Task EstablishTcpConnection(IComponentContext scope,bool condition)
    {
        if (condition)
        {
            _connectionService = scope.ResolveNamed<IConnectionService>("Server");
            _serverConnectionService = scope.Resolve<IServerConnectionService>();

            Console.WriteLine(
                $"Connection will be establish as server. Host: {InternetProtocol.GetCurrentIPv4Address()}:{_connectionParameter.Port}");
        }
        else
        {
            _connectionService = scope.ResolveNamed<IConnectionService>("Client");

            Console.WriteLine("Connection will be establish as client.\nPlease enter host ip and port.");
        }

        _connectionService.EstablishConnection();
        Console.WriteLine("Connection successfully established.");

        var receive = condition ? Task.Run(ReceiveMessagesMulti) : Task.Run(ReceiveMessagesAsync);

        Task send = Task.Run(SendMessagesAsync);

        await Task.WhenAll(receive, send);
    }

    private static async Task ReceiveMessagesAsync()
    {
        await foreach (var message in _connectionService.GetMessagesAsync())
        {
            Console.WriteLine(message);
        }
    }
    private static async Task ReceiveMessagesMulti()
    {
        await foreach (var client in _serverConnectionService.AcceptClientsAsync())
        {
            //await Task.Run(() => ReceiveMessagesMultiClientsAsync(client));
            _ = Task.Run(() => ReceiveMessagesMultiClientsAsync(client));
        }
    }
    private static async Task ReceiveMessagesMultiClientsAsync(Socket client)
    {
        await foreach (var message in _serverConnectionService.ReceiveMessagesMultiClientsAsync(client))
        {
            Console.WriteLine(message);
        }
    }
    
    private static async Task SendMessagesAsync()
    {
        while (true)
        {
            var message = Console.ReadLine();
            if (message != null)
                await _connectionService.SendMessageAsync(message);
        }
    }

    private static ILifetimeScope Scope()
    {
        var builder = new ContainerBuilder(); 
    
        builder.RegisterModule(new AutofacBusinessModule());
    
        var container = builder.Build();

        return container.BeginLifetimeScope();
    }

    private static bool ResponseControl(string response)
    {
        HashSet<string> responses = new() { "global", "local" };
        return responses.Contains(response);
    }
    private static bool GlobalResponseControl(string response)
    {
        HashSet<string> responses = new() { "server", "client" };
        return responses.Contains(response);
    }
}
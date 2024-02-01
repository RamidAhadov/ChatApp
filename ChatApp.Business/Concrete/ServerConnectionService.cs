using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using ChatApp.Business.Abstraction;
using ChatApp.Configuration.Abstraction;
using ChatApp.Core.Exceptions;
using ChatApp.Core.Utilities.Protocols;

namespace ChatApp.Business.Concrete;

public class ServerConnectionService:IServerConnectionService
{
    private readonly IConnectionParameter _parameter;
#pragma warning disable CS0649
    private List<Socket>? _clients;
    private TcpListener? _tcpListener;
#pragma warning restore CS0649
    //private static object _lockObject = new object();

    public ServerConnectionService(IConnectionParameter parameter)
    {
        _parameter = parameter;
    }
    
    public void EstablishConnection()
    {
        var ipAddress = InternetProtocol.GetCurrentIPv4Address();
        int port = _parameter.Port;
        
        _tcpListener = new TcpListener(ipAddress, port);
        _tcpListener.Start();

        _clients = new List<Socket>();
    }

    public async Task<string?> SendMessageAsync(string message)
    {
        message = ((IPEndPoint)_tcpListener.LocalEndpoint).Address.MapToIPv4() + ": " + message;
        
        await SendToClientsAsync(message);
        
        return message;
    }
    
    public async IAsyncEnumerable<string?> GetMessagesAsync()
    {
        var client = await _tcpListener.AcceptSocketAsync();

        await foreach (var message in GetMessagesAfterAccept(client))
        {
            yield return message;
        }
    }

    private async IAsyncEnumerable<Socket> AcceptedSockets()
    {
        while (true)
        {
            Console.WriteLine("Waiting for new client..."); //Checkpoint
            yield return await _tcpListener.AcceptSocketAsync();
        }
    }

    private async IAsyncEnumerable<string?> GetMessagesAfterAccept(Socket client)
    {
        Console.WriteLine("New client accepted!");
        string endPointIP = TransmissionControlProtocol.GetEndpointFromClient(client);

        while (client.Connected)
        {
            var message = await Task.Run(() => ReceiveMessages(client));
            message = endPointIP + ": " + message;

            yield return message;
        }

        _clients.Remove(client);
        yield return endPointIP + " disconnected.";
    }

    private async Task<string?> ReceiveMessages(Socket client)
    {
        try
        {
            AddClients(client);

            byte[] data = new byte[100];

            int bytesRead = await client.ReceiveAsync(data, SocketFlags.None);

            if (bytesRead > 0)
            {
                string result = Encoding.UTF8.GetString(data, 0, bytesRead);
                await SendToClientsAsync(result, client);
                return result;
            }

            await client.DisconnectAsync(false);
            return "null";
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
            client.Dispose();
            return null;
        }
    }


    private void AddClients(Socket client)
    {
        if (!_clients.Contains(client))
        {
            _clients.Add(client);
        }
    }
    
    private async Task SendToClientsAsync(string message)
    {
        var data = Encoding.UTF8.GetBytes(message);
        foreach (var client in _clients!)
        {
            if (client.Connected)
            {
                await client.SendAsync(data);
            }
        }
    }
    private async Task SendToClientsAsync(string message,Socket sender)
    {
        var data = Encoding.UTF8.GetBytes(message);
        foreach (var client in _clients!)
        {
            if (client.Connected && client != sender)
            {
                await client.SendAsync(data);
            }
        }
    }
}
using System.Net.Sockets;
using System.Text;
using ChatApp.Business.Abstraction;
using ChatApp.Configuration.Abstraction;
using ChatApp.Core.Utilities.Protocols;

namespace ChatApp.Business.Concrete;

public class ServerConnectionService:IServerConnectionService
{
    private readonly IConnectionParameter _parameter;
#pragma warning disable CS0649
    private List<Socket>? _clients;
    private TcpListener? _tcpListener;
#pragma warning restore CS0649

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
        byte[] data = Encoding.UTF8.GetBytes(message);
        foreach (var client in _clients!)
        {
            // if (client != sender)
            // {
                await client.SendAsync(data);
            //}
        }
        
        return message;
    }
    public async Task<string?> GetMessagesAsync()
    {
        try
        {
            using var client = await _tcpListener.AcceptSocketAsync();
            Console.WriteLine("New connection accepted");
            
            AddClients(client);

            byte[] data = new byte[100];
        
            await client.ReceiveAsync(data);

            string result = Encoding.UTF8.GetString(data);
        
            client.Close();

            return result;
        }
        catch (Exception e)
        {
            return $"An error occured: {e.InnerException.Message}";
        }
        
    }

    private void AddClients(Socket client)
    {
        if (!_clients.Contains(client))
        {
            _clients.Add(client);
        }
    }
}
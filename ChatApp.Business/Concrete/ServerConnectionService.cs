using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
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
        var data = Encoding.UTF8.GetBytes(message);
        foreach (var client in _clients!)
        {
            await client.SendAsync(data);
        }
        
        return message;
    }

    
    //Server do not accepts second client.
    //Might be due to multithreading issue.
    public async IAsyncEnumerable<string?> GetMessagesAsync()
    {
        bool connected = true;
        using (var client = await _tcpListener.AcceptSocketAsync())
        {
            while (connected)
            {
                var message = await Task.Run(() => ReceiveMessages(client));
                if (message == null)
                {
                    //Try to remove "connected" variable
                    _clients.Remove(client);
                    connected = false; break;
                }
                else
                {
                    yield return message;
                }
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private async Task<string?> ReceiveMessages(Socket client)
    {
        try
        {
            AddClients(client);

            byte[] data = new byte[100];

            int bytesRead = await client.ReceiveAsync(data, SocketFlags.None);
            Console.WriteLine("Received async!");

            if (bytesRead > 0)
            {
                string result = Encoding.UTF8.GetString(data, 0, bytesRead);
                //Console.WriteLine(result);
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
}
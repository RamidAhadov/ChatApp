using System.Net;
using System.Net.Sockets;
using System.Text;
using ChatApp.Business.Abstraction;
using ChatApp.Configuration.Abstraction;

namespace ChatApp.Business.Concrete;

public class ServerConnectionService:IConnectionService
{
    private readonly IConnectionParameter _parameter;
    private static List<TcpClient> _clients;
    static TcpListener _tcpListener;

    public ServerConnectionService(IConnectionParameter parameter)
    {
        _parameter = parameter;
    }
    
    public async Task EstablishConnection()
    {
        IPAddress ipAddress = IPAddress.Parse(_parameter.Ip);
        int port = _parameter.Port;
        
        _tcpListener = new TcpListener(ipAddress, port);
        _tcpListener.Start();

        Console.WriteLine($"Server started on {ipAddress}:{port}");
        
        while (true)
        {
            TcpClient client = await _tcpListener.AcceptTcpClientAsync();
            _clients.Add(client);

            await Task.Run(() => HandleClient(client));
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    private static async Task HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received data from client: {dataReceived}");

            //Broadcast(dataReceived, client);
        }
        _clients.Remove(client);
        client.Close();
    }
    
    private static void Broadcast(string message, TcpClient senderClient)
    {
        foreach (var client in _clients)
        {
            if (client != senderClient)
            {
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
    }
    
    
}
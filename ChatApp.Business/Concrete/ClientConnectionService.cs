using System.Net.Sockets;
using System.Text;
using ChatApp.Business.Abstraction;
using ChatApp.Configuration.Abstraction;

namespace ChatApp.Business.Concrete;

public class ClientConnectionService:IClientConnectionService
{
    private TcpClient _client;
    private IConnectionParameter _connectionParameter;

    public ClientConnectionService(IConnectionParameter connectionParameter)
    {
        _connectionParameter = connectionParameter;
    }

    public void EstablishConnection()
    {
        Console.WriteLine("IP:");
        string? serverIp = Console.ReadLine();
        Console.WriteLine("Port:");
        int port = int.Parse(Console.ReadLine() ?? string.Empty);

        if (serverIp == null)
        {
            throw new ArgumentNullException(nameof(serverIp));
        }
        _client = new TcpClient(serverIp, port);
    }

    public async Task<string?> GetMessagesAsync()
    {
        Console.WriteLine("GetMessagesAsync");
        return await ReceiveData(_client);
    }

    public async Task<string?> SendMessageAsync(string message)
    {
        Console.WriteLine("SendMessagesAsync");
        await SendData(_client,message);
        return message;
    }

    private static async Task<string> ReceiveData(TcpClient client)
    {
        var stream = client.GetStream();
        
        var reader = new StreamReader(stream, Encoding.UTF8);
    
        string? receivedData = null;
        
        while (true)
        {
            char[] buffer = new char[1024];
            int bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length);
    
            if (bytesRead == 0)
            {
                continue;
            }
    
            receivedData = new string(buffer, 0, bytesRead);
            break;
        }
    
        Console.WriteLine("Message received: " + receivedData);
        return receivedData;
    }
    
    

    static async Task SendData(TcpClient client,string message)
    {
        Console.WriteLine("Enter a message: ");

        NetworkStream stream = client.GetStream();
        byte[] data = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(data, 0, data.Length);
        await stream.FlushAsync();
        Console.WriteLine("Bura isledi :)");
        
    }
}
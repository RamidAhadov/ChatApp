using System.Net.Sockets;
using System.Text;
using ChatApp.Business.Abstraction;
using ChatApp.Configuration.Abstraction;

namespace ChatApp.Business.Concrete;

public class ClientConnectionService:IClientConnectionService
{
    private IConnectionParameter _connectionParameter;

    public ClientConnectionService(IConnectionParameter connectionParameter)
    {
        _connectionParameter = connectionParameter;
    }

    //Split receive data and send data methods to 2 different method.
    public async Task EstablishConnection()
    {
        string serverIp = _connectionParameter.Ip;
        int port = _connectionParameter.Port;
        
        while(true)
        {
            using (TcpClient client = new TcpClient(serverIp, port))
            {
                Console.WriteLine($"Connected to server at {serverIp}:{port}");

                await Task.Run(() => ReceiveData(client));
                await Task.Run(() => SendData(client));
            }
        }

        Console.ReadLine();
    }
    
    static async Task ReceiveData(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received data from server: {dataReceived}");
        }
    }

    static async Task SendData(TcpClient client)
    {
        while (true)
        {
            Console.Write("Enter a message: ");
            string message = Console.ReadLine();

            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        }
    }
}
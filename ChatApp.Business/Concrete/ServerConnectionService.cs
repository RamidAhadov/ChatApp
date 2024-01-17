using System.Net;
using System.Net.NetworkInformation;
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
        
        //var ipAddr = LocalIpAddress();
        //var publicIp = await GetPublicIpAddressAsync();
        _tcpListener = new TcpListener(ipAddress, port);
        _tcpListener.Start();

        Console.WriteLine($"Server started on {ipAddress}:{port}");

        while (true)
        {
            await Task.Run(ReadDataFromClientAsync);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private async Task ReadDataFromClientAsync()
    {
        Socket client = await _tcpListener.AcceptSocketAsync();
        Console.WriteLine("New connection accepted");
        while (true)
        {

            byte[] data = new byte[100];
            int size = await client.ReceiveAsync(data);
            for (int i = 0; i < size; i++)
            {
                Console.Write(Convert.ToChar(data[i]));
            }

            Console.WriteLine();
        }
    }

    private static IPAddress? LocalIpAddress()
    {
        if (!NetworkInterface.GetIsNetworkAvailable())
            return null;

        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ipAddress in host.AddressList)
        {
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                return ipAddress;
            }
        }

        return null;
        //return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
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

    static async Task<IPAddress> GetPublicIpAddressAsync()
    {
        using (var client = new HttpClient())
        {
            try
            {
                string response = await client.GetStringAsync("https://httpbin.org/ip");
                string publicIp = response.Split('"')[3];
                return IPAddress.Parse(publicIp);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    
    
}
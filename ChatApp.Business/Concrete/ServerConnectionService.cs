using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using ChatApp.Business.Abstraction;
using ChatApp.Configuration.Abstraction;

namespace ChatApp.Business.Concrete;

public class ServerConnectionService:IServerConnectionService
{
    private readonly IConnectionParameter _parameter;
    private static List<TcpClient> _clients;

    private static TcpListener? _tcpListener;
    //private static object lockObject = new object();

    public ServerConnectionService(IConnectionParameter parameter)
    {
        _parameter = parameter;
    }
    
    public void EstablishConnection()
    {
        string ipv4 = GetIPv4Address();
        IPAddress ipAddress = IPAddress.Parse(ipv4);
        int port = _parameter.Port;
        
        _tcpListener = new TcpListener(ipAddress, port);
        _tcpListener.Start();

        Console.WriteLine($"Server started on {ipAddress}:{port}");
    }

    public async Task<string?> SendMessageAsync(string message, TcpClient sender)
    {
        foreach (var client in _clients)
        {
            if (client != sender)
            {
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message);
                
                await stream.WriteAsync(data, 0, data.Length); //Test with multi user
            }
        }
        
        return message;
    }
    public async Task<string?> GetMessagesAsync()
    {
        try
        {
            Socket client = await _tcpListener.AcceptSocketAsync();
            Console.WriteLine("New connection accepted");
            byte[] data = new byte[100];
        
            await client.ReceiveAsync(data);

            string result = Encoding.UTF8.GetString(data);
        
            client.Close();

            return result;
        }
        catch (Exception e)
        {
            return $"An error occured: {e.Message}";
        }
        
    }

    static string GetIPv4Address()
    {
        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface networkInterface in networkInterfaces)
        {
            if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                UnicastIPAddressInformationCollection unicastAddresses = ipProperties.UnicastAddresses;

                foreach (UnicastIPAddressInformation unicastAddress in unicastAddresses)
                {
                    if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return unicastAddress.Address.ToString();
                    }
                }
            }
        }

        return null;
    }
    
}
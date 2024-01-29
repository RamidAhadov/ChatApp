using System.Net;
using System.Net.Sockets;

namespace ChatApp.Core.Utilities.Protocols;

public class TransmissionControlProtocol
{
    public static string GetEndpointFromClient(Socket client)
    {
        return ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
    }
}
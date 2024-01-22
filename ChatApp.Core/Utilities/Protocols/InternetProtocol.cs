using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using ChatApp.Core.Exceptions;

namespace ChatApp.Core.Utilities.Protocols;

public static class InternetProtocol
{
    public static IPAddress GetCurrentIPv4Address()
    {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var networkInterface in networkInterfaces)
        {
            if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                var ipProperties = networkInterface.GetIPProperties();
                var uniCastAddresses = ipProperties.UnicastAddresses;

                foreach (var uniCastAddress in uniCastAddresses)
                {
                    if (uniCastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IPAddress.Parse(uniCastAddress.Address.ToString());
                    }
                }
            }
        }

        throw new InternetProtocolException();
    }
}
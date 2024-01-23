using System.ComponentModel;
using System.Net.Sockets;
using ChatApp.Business.Abstraction;
using ChatApp.Core.Utilities.Protocols;

namespace ChatApp.Business.Concrete;

public class CheckPortService:ICheckPortService
{
    [Description("If true, no one listening to the port.")]
    public bool IsListening(int portNumber)
    {
        var ipAddress = InternetProtocol.GetCurrentIPv4Address();
        try
        {
            var tcpListener = new TcpListener(ipAddress, portNumber);
            tcpListener.Start();
            tcpListener.Stop();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
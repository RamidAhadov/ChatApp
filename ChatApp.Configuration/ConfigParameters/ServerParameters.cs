using ChatApp.Configuration.Abstraction;

namespace ChatApp.Configuration.ConfigParameters;

public class ServerParameters:IConnectionParameter
{
    public ServerParameters(string ip, int port)
    {
        Ip = ip;
        Port = port;
    }

    public string Ip { get; }
    public int Port { get; }
}
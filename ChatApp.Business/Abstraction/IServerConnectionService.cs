using System.Net.Sockets;

namespace ChatApp.Business.Abstraction;

public interface IServerConnectionService
{
    void EstablishConnection();
    Task<string?> GetMessagesAsync();
    Task<string?> SendMessageAsync(string message, TcpClient sender);
}
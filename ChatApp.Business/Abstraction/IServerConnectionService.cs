using System.Net.Sockets;

namespace ChatApp.Business.Abstraction;

public interface IServerConnectionService:IConnectionService
{
    IAsyncEnumerable<Socket> AcceptClientsAsync();
    IAsyncEnumerable<string> ReceiveMessagesMultiClientsAsync(Socket client);
}
namespace ChatApp.Business.Abstraction;

public interface IConnectionService
{
    void EstablishConnection();
    IAsyncEnumerable<string?> GetMessagesAsync();
    Task<string?> SendMessageAsync(string message);
}
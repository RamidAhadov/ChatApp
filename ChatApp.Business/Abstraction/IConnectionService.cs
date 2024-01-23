namespace ChatApp.Business.Abstraction;

public interface IConnectionService
{
    void EstablishConnection();
    Task<string?> GetMessagesAsync();
    Task<string?> SendMessageAsync(string message);
}
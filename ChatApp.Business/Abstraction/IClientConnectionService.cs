namespace ChatApp.Business.Abstraction;

public interface IClientConnectionService
{
    Task EstablishConnection();
}
namespace ChatApp.Business.Abstraction;

public interface ICheckPortService
{
    bool IsListening(int portNumber);
}
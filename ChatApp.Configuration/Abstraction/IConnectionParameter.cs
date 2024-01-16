namespace ChatApp.Configuration.Abstraction;

public interface IConnectionParameter
{
    string Ip { get; }
    int Port { get; }
}
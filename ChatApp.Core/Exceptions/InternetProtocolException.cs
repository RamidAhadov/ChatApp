namespace ChatApp.Core.Exceptions;

public class InternetProtocolException:Exception
{
    public InternetProtocolException()
    {
        
    }
    public InternetProtocolException(string message):base(message)
    {
        
    }
    public InternetProtocolException(string message,Exception innerException):base(message,innerException)
    {
        
    }
}
using ChatApp.Configuration.Abstraction;
using ChatApp.Configuration.ConfigParameters;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Configuration;

public class Configs:IConfig
{
    private IConfiguration Configuration { get; }
    private readonly TestText? _testText;
    private readonly ServerParameters? _serverParameters;

    public Configs(IConfiguration configuration)
    {
        Configuration = configuration;
        _testText = Configuration.GetSection("TestTextNotFound").Get<TestText>();
        _serverParameters = Configuration.GetSection("ServerParameters").Get<ServerParameters>();
    }

    public TestText? TestText()
    {
        return _testText;
    }

    public ServerParameters? ServerParameters()
    {
        return _serverParameters;
    }
}
using ChatApp.Configuration.Abstraction;
using ChatApp.Configuration.ConfigParameters;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Configuration;

public class Configs:IConfig
{
    private IConfiguration Configuration { get; }
    private readonly TestText? _testText;

    public Configs(IConfiguration configuration)
    {
        Configuration = configuration;
        _testText = Configuration.GetSection("TestTextNotFound").Get<TestText>();
    }

    public TestText? TestText()
    {
        return _testText;
    }
}
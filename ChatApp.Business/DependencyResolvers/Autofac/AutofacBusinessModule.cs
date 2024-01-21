using Autofac;
using ChatApp.Business.Abstraction;
using ChatApp.Business.Concrete;
using ChatApp.Configuration;
using ChatApp.Configuration.Abstraction;
using ChatApp.Configuration.ConfigParameters;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Business.DependencyResolvers.Autofac;

public class AutofacBusinessModule:Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(c => new ConfigurationBuilder()
                .SetBasePath("/Users/macbook/RiderProjects/ChatApp/ChatApp.Configuration")
                .AddJsonFile("appsettings.json")
                .Build())
            .As<IConfiguration>()
            .SingleInstance();

        builder.RegisterType<Configs>().As<IConfig>();
        builder.RegisterType<Configs>();

        builder.Register(c =>
            {
                var configuration = c.Resolve<IConfiguration>();
                var serverParameters = configuration.GetSection("ServerParameters").Get<ServerParameters>();
                return serverParameters;
            })
            .As<IConnectionParameter>().SingleInstance();
        
        builder.RegisterType<ServerConnectionService>().As<IServerConnectionService>();
        builder.RegisterType<ServerConnectionService>();
        
        builder.RegisterType<ClientConnectionService>().As<IClientConnectionService>();
    }
}
using Autofac;
using ChatApp.Configuration;
using ChatApp.Configuration.Abstraction;
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
    }
}
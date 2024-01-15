using System.Net;
using Autofac;
using ChatApp.Business.DependencyResolvers.Autofac;
using ChatApp.Configuration;

var builder = new ContainerBuilder(); 

builder.RegisterModule(new AutofacBusinessModule());

var container = builder.Build();

var scope = container.BeginLifetimeScope();

var path = scope.Resolve<Configs>();

Console.WriteLine(path.TestText()?.Name);
Console.WriteLine(path.TestText()?.Surname); 
Console.WriteLine(path.TestText()?.Family);

Console.WriteLine(IPAddress.Any);
using System.Net;
using System.Runtime.CompilerServices;
using Autofac;
using ChatApp.Business.DependencyResolvers.Autofac;
using ChatApp.Configuration;

var builder = new ContainerBuilder(); 

builder.RegisterModule(new AutofacBusinessModule());

var container = builder.Build();

var scope = container.BeginLifetimeScope();

var path = scope.Resolve<Configs>();

await SendRequest();

// Console.WriteLine(path.TestText()?.Name);
// Console.WriteLine(path.TestText()?.Surname); 
// Console.WriteLine(path.TestText()?.Family);

static async Task SendRequest()
{
    const string url = "http://localhost:5195/api/Test/getString";
    
    using (HttpClient client = new HttpClient())
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        HttpResponseMessage response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        else
        {
            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }
}



//Console.WriteLine(IPAddress.Any);
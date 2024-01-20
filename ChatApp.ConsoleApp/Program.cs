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

Task.Run(ReadLineAsync);

await ReceiveMessages();

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

static async Task ReceiveMessagesAsync()
{
    const string url = "http://localhost:5195/api/Test/getMessagesAsync";
    using (var client = new HttpClient())
    {
        //client.Timeout = TimeSpan.FromSeconds(500);
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        var response = await client.SendAsync(request);
        
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

async Task ReceiveMessages()
{
    while (true)
    {
        //Timeout (100 sec)
        await ReceiveMessagesAsync();
    }
}

void ReadLine()
{
    while (true)
    {
        Console.ReadLine();
    }
}

Task ReadLineAsync()
{
    while (true)
    {
        Console.ReadLine();
    }
}


//Console.WriteLine(IPAddress.Any);
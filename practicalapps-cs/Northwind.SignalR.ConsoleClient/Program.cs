using Microsoft.AspNetCore.SignalR.Client;
using Northwind.Chat.Models;
using static System.Console;

Write("Enter a username: ");
string? username = ReadLine();

Write("Enter your groups: ");
string? groups = ReadLine();

HubConnection hubConnection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7273/chat", (opts) =>
{
    opts.HttpMessageHandlerFactory = (message) =>
    {
        if (message is HttpClientHandler clientHandler)
            // always verify the SSL certificate
            clientHandler.ServerCertificateCustomValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => { return true; };
        return message;
    };
})
    .Build();

hubConnection.On<MessageModel>("ReceiveMessage", message =>
{
    WriteLine($"{message.From} says {message.Body} (sent to {message.To}");
});

await hubConnection.StartAsync();

WriteLine("Successfully registered");
WriteLine("Listening... (press ENTER to stop)");
ReadLine();
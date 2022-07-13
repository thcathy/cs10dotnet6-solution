using System.Net.NetworkInformation;
using Microsoft.AspNetCore.SignalR;
using Northwind.Chat.Models;

namespace Northwind.Mvc.Hubs;

public class ChatHub : Hub {
    private static Dictionary<string, string> users = new();

    public async Task Register(RegisterModel model) {
        users[model.Username] = Context.ConnectionId;

        foreach (string group in model.Groups.Split(",")) {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }
    }

    public async Task SendMessage(MessageModel message) {
        MessageModel reply = new()
        {
            From = message.From,
            Body = message.Body
        };
        IClientProxy proxy;

        switch (message.ToType) {
            case "User":
                string connectionId = users[message.To];
                reply.To = $"{message.To} [{connectionId}]";
                proxy = Clients.Client(connectionId);
                break;
            case "Group":
                reply.To = $"Group: {message.To}";
                proxy = Clients.Group(message.To);
                break;
            default:
                reply.To = "Everyone";
                proxy = Clients.All;
                break;
        }

        await proxy.SendAsync("ReceiveMessage", reply);
    }
}
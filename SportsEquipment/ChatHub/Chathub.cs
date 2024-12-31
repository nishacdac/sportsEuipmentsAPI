using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SportsEquipment.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class ChatHub : Hub
{
    private static readonly Dictionary<string, List<ChatMessage>> userMessages = new Dictionary<string, List<ChatMessage>>();

    public async Task SendMessage(string user, string message)
    {
        try
        {
            var chatMessage = new ChatMessage { User = user, Message = message, Read = false };
            if (!userMessages.ContainsKey(user))
            {
                userMessages[user] = new List<ChatMessage>();
            }
            userMessages[user].Add(chatMessage);

            await Clients.All.SendAsync("ReceiveMessage", user, message, false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SendMessage: {ex.Message}");
            throw;
        }
    }

    public async Task MessageSeen(string user, string message)
    {
        await Clients.All.SendAsync("MessageSeen", user, message);
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        // Handle disconnect
        userMessages.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public override async Task OnConnectedAsync()
    {
        var userName = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userName) && userMessages.ContainsKey(userName))
        {
            foreach (var message in userMessages[userName])
            {
                await Clients.Caller.SendAsync("ReceiveMessage", message.User, message.Message, message.Read);
            }
        }
        await base.OnConnectedAsync();
    }
}

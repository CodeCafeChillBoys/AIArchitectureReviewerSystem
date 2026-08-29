using Microsoft.AspNetCore.SignalR;

namespace DiagramManager.Infrastructure.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
}

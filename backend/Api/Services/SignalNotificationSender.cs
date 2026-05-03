using InteractHub.Application.Interfaces.Services;
using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using InteractHub.Api.Hubs; // Dẫn link tới thư mục Hub của API

namespace InteractHub.Api.Services;

public class SignalRNotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationSender(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(string userId, Notification notification)
    {
        // Thực hiện lệnh bắn SignalR tại đây
        await _hubContext.Clients.User(userId).SendAsync("ReceiveNewNotification", notification);
    }
}
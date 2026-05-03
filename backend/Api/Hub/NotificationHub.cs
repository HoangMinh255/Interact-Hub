using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace InteractHub.Api.Hubs;

[Authorize] // Yêu cầu phải có token JWT mới được kết nối
public class NotificationHub : Hub
{

}
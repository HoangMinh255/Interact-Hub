using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace InteractHub.Api.Hubs;

[Authorize] // Yêu cầu phải có token JWT mới được kết nối
public class NotificationHub : Hub
{
    // Bạn không cần viết hàm gì ở đây nếu chỉ Server đẩy (push) xuống Client.
    // Việc kết nối (OnConnectedAsync) và ngắt kết nối (OnDisconnectedAsync) SignalR tự quản lý.
}
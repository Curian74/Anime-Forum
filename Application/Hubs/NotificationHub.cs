using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace Application.Hubs
{
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> UserGroups
          = new ConcurrentDictionary<string, HashSet<string>>();
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                
                Console.WriteLine("========================================");
                Console.WriteLine("Connection id: " + Context.ConnectionId);
                Console.WriteLine($"✅ User {userId} joined group {userId}");
            }
            else
            {

                Console.WriteLine("========================================");
                Console.WriteLine("❌ User ID is null, cannot join group.");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId) && UserGroups.ContainsKey(userId))
            {
                UserGroups[userId].Remove(Context.ConnectionId);
                if (UserGroups[userId].Count == 0)
                {
                    UserGroups.TryRemove(userId, out _);
                }

                Console.WriteLine($"❌ User {userId} left group {userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}

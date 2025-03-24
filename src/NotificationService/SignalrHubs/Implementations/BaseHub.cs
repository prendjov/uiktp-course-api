using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService.SignalrHubs.Implementations;

[Authorize(Policy = "SignalrAuth")]
public class BaseHub<THub> : Hub<THub>
    where THub : class
{
    public BaseHub()
    {
    }   
    public override Task OnConnectedAsync()
    {
        var context = Context;
        return base.OnConnectedAsync();
    }

    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }

    public virtual Task JoinGroup(string groupName)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, groupName.ToLower());
    }

    public async Task<string> JoinPrivateGroup(string groupName)
    {
        var privateGroupName = $"{groupName}_{GetUserClaimValue("id")}";
        await JoinGroup(privateGroupName);
        return privateGroupName;
    }

    public virtual Task LeaveGroup(string groupName)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName.ToLower());
    }

    public async Task<string> LeavePrivateGroup(string groupName)
    {
        var privateGroupName = $"{groupName}_{GetUserClaimValue("id")}";
        await LeaveGroup(privateGroupName);

        return privateGroupName;
    }

    private string? GetUserClaimValue(string claimName)
    {
        return Context.User!.Claims.Where(c => c.Type == claimName).Select(c => c.Value).FirstOrDefault();
    }

    public int? GetUserId()
    {
        var id = GetUserClaimValue("id");

        if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int userId))
        {
            return userId;
        }
        return null;
    }
}

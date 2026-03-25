using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace CRMBanks.Web.Hubs;

public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        // Get the user ID from the JWT NameIdentifier claim
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}

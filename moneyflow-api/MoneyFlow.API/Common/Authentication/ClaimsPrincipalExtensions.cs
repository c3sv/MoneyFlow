using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MoneyFlow.API.Common.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal principal)
    {
        var userIdValue = principal.FindFirstValue(
            JwtRegisteredClaimNames.Sub);

        if (!long.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedAccessException(
                "The authenticated user identifier is invalid.");
        }

        return userId;
    }
}
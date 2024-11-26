using System.Security.Claims;

namespace AuthTest.Extensions
{
    internal static class ClaimTypesExtension
    {
        internal static Guid UserId(this ClaimsPrincipal user)
             => Guid.NewGuid();

        internal static string UserName(this ClaimsPrincipal user)
            => user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;

        internal static string UserEmail(this ClaimsPrincipal user)
           => user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? string.Empty;
    }
}

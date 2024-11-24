using System.Security.Claims;
using System.Xml.Linq;

namespace AuthTest.Extensions
{
    public static class ClaimTypesExtension
    {
        public static Guid Id(this ClaimsPrincipal user)
        {
            return Guid.NewGuid();
        }
        public static string Name(this ClaimsPrincipal user)
        {
            try
            {
                return user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string Email(this ClaimsPrincipal user)
        {
            try
            {
                return user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}

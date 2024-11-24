using System.Security.Claims;
using System.Xml.Linq;

namespace AuthTest.Extensions
{
    public static class ClaimTypesExtension
    {
        public static int Id(this ClaimsPrincipal user)
        {
            var idClaim = user.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            try
            {
                return Convert.ToInt32(idClaim);
            }
            catch (FormatException)
            {
                Console.WriteLine("Erro: Valor de ID inválido na claim.");
                return 0; 
            }
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

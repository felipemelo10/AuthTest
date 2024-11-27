using AuthTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthTest.Data;

namespace AuthAspNet.Services;

public class AuthService
{
    private readonly AuthDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthService(AuthDbContext dbContext ,IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<string> CreateTokenAsync(string userName, string password)
    {
        //Use AsNoTranking pra o ef core não rastrear o dado
        var user = await _dbContext.Users.AsNoTracking().Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email.Equals(userName, StringComparison.CurrentCultureIgnoreCase));

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            //Use notifications
            throw new Exception("Usuário/senha inválidos.");


        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(2),
            Subject = GenerateClaims(user)
        };
        //Crie a variável o mais proxima possivel de onde vai ser usada
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaims(User user)
    {
        var claimsIdentity = new ClaimsIdentity();

        claimsIdentity.AddClaim(new Claim("id", user.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        
        //adiciona as claims do roles
        foreach(var userRole in user.UserRoles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, userRole.Role.Name));
        }

        return claimsIdentity;
    }
}

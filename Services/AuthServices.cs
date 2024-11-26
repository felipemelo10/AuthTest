using AuthTest.Data;
using AuthTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AuthAspNet.Services
{
    public class AuthService
    {
        private readonly AuthDbContext _dbContext;
        private readonly IConfiguration _configuration; // para ler PrivateKey de appsettings.json

        public AuthService(AuthDbContext dbContext ,IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<string> CreateTokenAsync(string userName, string password)
        {
            var getuser = await _dbContext.Users.ToListAsync();

            var user = await _dbContext.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u=> u.Email.ToLower() == userName.ToLower()); //busca pelo email

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new Exception("Usuário/senha inválidos.");
            }

            var handler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); //ler PrivateKey de appsettings.json

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256);

            //informações do token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(2),
                Subject = GenerateClaims(user)
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        private static ClaimsIdentity GenerateClaims(User user)
        {
            var claimsIdentity = new ClaimsIdentity();

            claimsIdentity.AddClaim(new Claim("id", user.Id.ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            
            //adiciona as clams do roles
            foreach(var userRole in user.UserRoles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            return claimsIdentity;
        }
    }
}

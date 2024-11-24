using AuthAspNet;
using AuthAspNet.Models;
using AuthAspNet.Services;
using AuthTest.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<AuthService>();

//importando os middlewares
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // a forma como vai interrogar a requisição, onde está o token etc
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters{
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.PrivateKey)), //obter e descriptar
        ValidateIssuer = false,
        ValidateAudience = false 
    };
}); //padrão será bearer

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("admin", p => p.RequireRole("admin"));
});


var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/login", (AuthService service) =>
{
    var user = new User
    {
        Name = "Felipe",
        Email = "felipe@gmail.com",
        Roles = new[] { "aluno" }
    };
        
    return service.Create(user);
}
);

app.MapGet("/restricted", (ClaimsPrincipal user) => new
{
    id = user.Id(),
    name = user.Name(),
    email = user.Email()
}).RequireAuthorization();

app.MapGet("/admin", () => "You have access to the system, administrator.").RequireAuthorization("admin"); //RequireAuthorization busca a política

app.Run();
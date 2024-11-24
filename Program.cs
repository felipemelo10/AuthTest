using AuthAspNet;
using AuthAspNet.Services;
using AuthTest.Data;
using AuthTest.Extensions;
using AuthTest.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte para ler configurações de appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AuthDb"))); 
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddTransient<AuthService>();


var configuration = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();
var key = configuration["Jwt:Key"];

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters{
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), //obter e descriptar
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

app.MapGet("/login", async (string email, string password, AuthService authService) =>
{
    var token = await authService.CreateTokenAsync(email, password);

    if (token != null)
        return Results.Ok(new { token });
    else
        return Results.Unauthorized();
}
);

app.MapGet("/restricted", (ClaimsPrincipal user) => new
{
    id = user.Id(),
    name = user.Name(),
    email = user.Email()
}).RequireAuthorization();

app.MapGet("/admin", () => "You have access to the system, administrator.").RequireAuthorization("admin");

app.Run();
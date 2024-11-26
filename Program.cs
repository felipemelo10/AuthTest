using AuthAspNet;
using AuthAspNet.Services;
using AuthTest.Data;
using AuthTest.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte para ler configurações de appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AuthDb")));

builder.Services.AddTransient<AuthService>(); // Injeção de dependência

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
{
    IConfiguration configuration = builder.Configuration;
    var key = configuration["Jwt:Key"];

    options.TokenValidationParameters = new TokenValidationParameters{
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

app.MapPost("/login", async (string email, string password, AuthService authService) =>
{
    var token = await authService.CreateTokenAsync(email, password);

    if (token != null)
        return Results.Ok(new { token });
    else
        return Results.Unauthorized();
}
);

app.MapGet("/admin", () => "You have access to the system, administrator.").RequireAuthorization("admin");

app.Run();
using AuthAspNet.Services;
using AuthTest.Data;
using AuthTest.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Acredito que por padr�o o ASP.NET j� coleque o arquivo como principal fonte de variavel de ambiente
//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


//Mandar pra um arquivo e chamar o m�todo est�tico.
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AuthDb")));

builder.Services.AddTransient<AuthService>(); 

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
{
    IConfiguration configuration = builder.Configuration;
    var key = configuration["Jwt:Key"];

    options.TokenValidationParameters = new TokenValidationParameters{
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), 
        ValidateIssuer = false,
        ValidateAudience = false
    };
}); 

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("admin", p => p.RequireRole("admin"));
});

//Add ao pipeline e a config Cors pra seguran�a do acesso da aplica��o
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", async (User user, AuthDbContext context, AuthService authService) =>
{
    //Use repository pattern para acesso a daddos
    await context.Users.AddAsync(user);
    //Use padr�o UoW pra consist�ncia dos dados
    await context.SaveChangesAsync();
    //precisa criar o usu�rio no banco antes de criar esse token
    var token = await authService.CreateTokenAsync(user.Email, user.Password);

    //Retorno mais limpo usando condi��o tern�ria sem if/else
    return token is not null ? Results.Ok(token) : Results.Unauthorized();
});

app.MapGet("/admin", () => "You have access to the system, administrator.").RequireAuthorization("admin");

app.Run();
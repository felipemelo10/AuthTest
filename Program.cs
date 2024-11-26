using AuthAspNet.Models;
using AuthAspNet.Services;
using AuthTest.Configuration;
using AuthTest.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

#region Application Configuration
builder.AddAuthConfig().AddDependencyInjection();

builder.Services.AddSwaggerGen();
#endregion

#region Middleware Pipeline Configuration
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
#endregion

app.MapPost("/login", (AuthService service, [FromBody] User user) =>
{
    var users = new User(user.Name, user.Email, user.Roles);
        
    return Results.Ok(service.Create(user));
}
);

app.MapGet("/restricted", (ClaimsPrincipal user) => new
{
    id = user.UserId(),
    name = user.UserName(),
    email = user.UserEmail()
}).RequireAuthorization();

app.MapGet("/admin", () => "You have access to the system, administrator.").RequireAuthorization("admin");

app.Run();
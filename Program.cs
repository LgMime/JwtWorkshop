using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "dev_secret_change_me";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdminOnly", p => p.RequireClaim(ClaimTypes.Role, "admin"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

var users = new[]
{
    new { Id = 1, Email = "admin@example.com", Password = "admin123", Role = "admin" },
    new { Id = 2, Email = "user@example.com", Password = "user123", Role = "user" }
};


app.MapPost("/login", ([FromBody] JsonElement body) =>
{
    if (!body.TryGetProperty("email", out var emailProp) ||
        !body.TryGetProperty("password", out var passProp))
    {
        return Results.BadRequest(new { error = "Missing email or password" });
    }

    string email = emailProp.GetString() ?? "";
    string password = passProp.GetString() ?? "";

    var u = users.FirstOrDefault(x => x.Email == email && x.Password == password);
    if (u is null)
        return Results.Unauthorized();

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, u.Id.ToString()), 
        new Claim(ClaimTypes.Role, u.Role)
    };

    var jwt = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(15),
        signingCredentials: creds
    );

    var token = new JwtSecurityTokenHandler().WriteToken(jwt);

    return Results.Json(new { access_token = token, token_type = "Bearer", expires_in = 900 });
});

app.MapGet("/profile", (ClaimsPrincipal user) =>
{
    var sub = user.FindFirstValue(ClaimTypes.NameIdentifier);
    var role = user.FindFirstValue(ClaimTypes.Role);
    return Results.Json(new { user_id = sub, role });
}).RequireAuthorization();


app.MapDelete("/users/{id:int}", (int id) =>
{
    return Results.Json(new { message = $"User {id} deleted (demo)" });
}).RequireAuthorization("AdminOnly");

app.Run();

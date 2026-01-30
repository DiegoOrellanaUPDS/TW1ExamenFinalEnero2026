using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================
// Cadena de conexión
// =====================
var connectionString =
    Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("Connection");

// =====================
// Servicios
// =====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure();
    })
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================
// SESSION (CRÍTICO PARA OAUTH)
// =====================
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".ExamenFinal.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// =====================
// AUTHENTICATION - DISCORD OAUTH2
// =====================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Discord";
})
.AddCookie()
.AddOAuth("Discord", options =>
{
    options.ClientId = "1466792412616593471";
    options.ClientSecret = "0jAFLXJJrhAFev9akB_emCLVVNTGV8JB";

    options.CallbackPath = "/api/auth/discord/callback";

    options.AuthorizationEndpoint = "https://discord.com/api/oauth2/authorize";
    options.TokenEndpoint = "https://discord.com/api/oauth2/token";
    options.UserInformationEndpoint = "https://discord.com/api/users/@me";

    options.Scope.Add("identify");
    options.SaveTokens = true;

    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");

    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                context.Options.UserInformationEndpoint
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", context.AccessToken);

            var response = await context.Backchannel.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var user = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync()
            );

            context.RunClaimActions(user.RootElement);
        }
    };
});

// =====================
// App
// =====================
var app = builder.Build();

// =====================
// Migraciones automáticas
// =====================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error aplicando migraciones: " + ex.Message);
    }
}

// =====================
// Middleware
// =====================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
});

app.UseCors("MyApp");

// ❌ NO usar HTTPS redirect en OAuth local
// app.UseHttpsRedirection();

// ⚠️ ORDEN CORRECTO (CRÍTICO)
app.UseSession();        // 1️⃣
app.UseAuthentication(); // 2️⃣
app.UseAuthorization();  // 3️⃣

app.MapControllers();

app.Run();

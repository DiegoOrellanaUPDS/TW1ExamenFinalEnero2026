<<<<<<< HEAD
using System;
using Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================
// Cadena de conexi�n
=======

using ExamenFinal.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// =====================
// Cadena de conexión
>>>>>>> feature/victorcox-entidad
// =====================
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                       ?? builder.Configuration.GetConnectionString("Connection");

// =====================
// Servicios
// =====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure();
    }));

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
builder.Services.AddHttpClient();
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();

// =====================
// App
// =====================
var app = builder.Build();

// =====================
<<<<<<< HEAD
// Migraciones autom�ticas
=======
// Migraciones automáticas
>>>>>>> feature/victorcox-entidad
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
<<<<<<< HEAD
    // Esto hace que Swagger est� en la ra�z
=======
    // Esto hace que Swagger esté en la raíz
>>>>>>> feature/victorcox-entidad
    //c.RoutePrefix = string.Empty;
});
app.UseCors("MyApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSession();
app.MapControllers();

<<<<<<< HEAD
app.Run();
=======
app.Run();
>>>>>>> feature/victorcox-entidad

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TiendaApi.Infrastructure; // donde tengas AppDbContext
using TiendaApi.Services;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin;
using FirebaseAdmin.Auth;

var builder = WebApplication.CreateBuilder(args);

var firebaseConfigPath = Path.Combine(AppContext.BaseDirectory, "firebase-key.json");

const string MyCorsPolicy = "AngularPolicy";

// Verifica si el archivo de configuración existe antes de inicializar
if (File.Exists(firebaseConfigPath))
{
    FirebaseApp.Create(new AppOptions()
    {
        // Usa GoogleCredential para cargar la clave privada desde el archivo JSON
        Credential = GoogleCredential.FromFile(firebaseConfigPath)
    });
    Console.WriteLine("Firebase Admin SDK inicializado correctamente.");
}
else
{
    Console.WriteLine("ADVERTENCIA: Archivo firebase-key.json no encontrado. La autenticación de Firebase fallará.");
    // Opcional: Podrías lanzar una excepción si las credenciales son obligatorias.
}


// 🔹 Configurar DbContext con SQL Server
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCorsPolicy,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// 🔹 Configurar DbContext con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 🔹 Aumentar límite de tamaño del cuerpo de la solicitud (para Base64)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100 MB
});

// 🔑 Configurar autenticación JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),
            NameClaimType = "id" // Usar el claim "id" para User.Identity.Name
        };
    });

// 🔑 servicios personalizados
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });



builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 🔹 Swagger con soporte para JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Tienda API", Version = "v1" });

    // Configuración para Authorize con Bearer Token
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Introduce: Bearer {tu token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// 🔹 Middleware de Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tienda API V1");
    c.RoutePrefix = "swagger"; // URL: http://localhost:5237/swagger
});

app.UseCors(MyCorsPolicy);

// 🔹 Servir archivos estáticos desde wwwroot (incluyendo uploads)
app.UseStaticFiles();

app.UseAuthentication(); // ⚡ Importante: antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();

// 🔹 Inicializar Base de Datos (Seed Roles)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}

app.Run();

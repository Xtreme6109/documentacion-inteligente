using DocumentacionInteligente.BackEnd.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Cors;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];


Console.WriteLine($"[DEBUG] SecretKey: {secretKey}");
Console.WriteLine($"[DEBUG] Issuer: {issuer}");
Console.WriteLine($"[DEBUG] Audience: {audience}");

//Cadena conexión.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       "Server=(localdb)\\DESKTOP-NFDMETJ\\MSSQLSERVER26;Database=DocumentacionInteligente;Trusted_Connection=True;MultipleActiveResultSets=true";

// Verificamos si la clave secreta es v�lida
if (string.IsNullOrEmpty(secretKey))
    throw new Exception("La clave secreta JWT no est� definida en appsettings.json (JwtSettings:SecretKey)");


// Configurar autenticaci�n JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //ValidateIssuer = true,
        //ValidateAudience = true,
        //ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Token inválido: " + context.Exception.Message);
            Console.WriteLine($"[DEBUG] SecretKey: {secretKey}");
            Console.WriteLine($"[DEBUG] Issuer: {issuer}");
            Console.WriteLine($"[DEBUG] Audience: {audience}");

            return Task.CompletedTask;
        }
    };
});


// Configurar pol�ticas de autorizaci�n
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireClaim("Rol", "Administrador"));
    options.AddPolicy("User", policy => policy.RequireClaim("Rol", "User"));
    //options.AddPolicy("EsAdministrador", policy => policy.RequireClaim("Rol", "Administrador"));
});

// Agregar DbContexts
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Agregar servicios
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configurar Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Documentacion Inteligente API",
        Version = "v1",
        Description = "API para la gestión de documentos y usuarios"
    });

    // Configuración de seguridad JWT para Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingrese el token JWT con el prefijo 'Bearer '"
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
            Array.Empty<string>()
        }
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:9000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configurar middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Documentos")),
    RequestPath = "/Documentos"
});
app.UseHttpsRedirection();


app.UseCors("AllowVueApp");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors("AllowVueApp");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

// Configurar endpoints para API, Razor y Vue
app.UseEndpoints(endpoints =>
{
    // API REST
    endpoints.MapControllers();

    endpoints.MapControllerRoute(
        name: "razor",
        pattern: "{controller}/{action}/{id?}");


    // Vue SPA fallback (solo si sirves Vue desde wwwroot)
    endpoints.MapFallbackToFile("index.html");
});

app.Run();

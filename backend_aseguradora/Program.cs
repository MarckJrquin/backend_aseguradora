using backend_aseguradora.Data;
using backend_aseguradora.Middlewares;
using backend_aseguradora.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de CORS para permitir cualquier origen, encabezado y m�todo.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});


// Agrega controladores al contenedor de servicios.
builder.Services.AddControllers();


// Agrega soporte para la exploraci�n de endpoints API.
builder.Services.AddEndpointsApiExplorer();


// Configuraci�n de Swagger para la documentaci�n de la API.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserRegistrationAPI", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        }
      },
      new string[] { }
    }
    });
});


// Configuraci�n del contexto de la base de datos con MySQL.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 36))));


// Configuraci�n de servicios personalizados.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IInsuranceTypeService, InsuranceTypeService>();
builder.Services.AddScoped<ICoverageService, CoverageService>();


// Configuraci�n de autenticaci�n JWT.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
                    };
                });


// Configuraci�n de autorizaci�n basada en roles.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("user"));
});


var app = builder.Build();


// Configuraci�n del pipeline de solicitud HTTP.
if (app.Environment.IsDevelopment())
{
    // Habilita Swagger en modo desarrollo.
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserRegistrationAPI v1"));
}


app.UseHttpsRedirection();


// Habilita CORS usando la pol�tica configurada.
app.UseCors("AllowAllOrigins");


// Middleware personalizado para roles.
app.UseMiddleware<RoleMiddleware>();


// Configuraci�n de autenticaci�n y autorizaci�n.
app.UseAuthentication();
app.UseAuthorization();


// Mapea controladores.
app.MapControllers();


// Ejecuta la aplicaci�n.
app.Run();

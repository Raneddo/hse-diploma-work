using ADSD.Backend.App;
using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Helpers;
using ADSD.Backend.App.Models;
using ADSD.Backend.App.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        var securityDefinition = new OpenApiSecurityScheme()
            {
                Name = "Bearer",
                BearerFormat = "JWT",
                Scheme = "bearer",
                Description = "Specify the authorization token.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
            };
        options.AddSecurityDefinition("jwt_auth", securityDefinition);

        // Make sure swagger UI requires a Bearer token specified
        var securityScheme = new OpenApiSecurityScheme()
        {
            Reference = new OpenApiReference()
            {
                Id = "jwt_auth",
                Type = ReferenceType.SecurityScheme
            }
        };
        var securityRequirements = new OpenApiSecurityRequirement()
        {
            {securityScheme, Array.Empty<string>()},
        };
        options.AddSecurityRequirement(securityRequirements);
    }
);
builder.Services.AddMemoryCache(item => { item.SizeLimit = 1000; });
builder.Services.AddScoped<SessionTokenDbClient>();
builder.Services.AddScoped<BasicAuthorizationHandler>();
builder.Services.AddScoped<AgendaService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PollService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<InfoService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<NotificationsService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<AppDbClient>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // укзывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.Issuer,

            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.Audience,
            // будет ли валидироваться время существования
            ValidateLifetime = true,

            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddControllersWithViews();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json");
}

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors(corsPolicyBuilder =>
{
    corsPolicyBuilder
        .WithOrigins("https://*.raneddo.ml", "http://localhost:3000", "https://mini-admin.netlify.app")
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
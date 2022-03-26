using ADSD.Backend.App;
using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Models;
using ADSD.Backend.App.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache(item => { item.SizeLimit = 1000; });
builder.Services.AddScoped<SessionTokenDbClient>();
builder.Services.AddScoped<BasicAuthorizationHandler>();
builder.Services.AddScoped<AgendaService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PollService>();
builder.Services.AddScoped<AuthService>();
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
builder.Services.AddMvc();

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
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
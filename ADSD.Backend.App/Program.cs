using System.Net;
using ADSD.Backend.App;
using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Services;
using Microsoft.AspNetCore.Authorization;

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
builder.Services.AddScoped<AppDbClient>();
// builder.Services.AddScoped<IAuthorizationHandler, BasicAuthorizationHandler>();
//
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("basic", 
//         policy => policy.Requirements.Add(new BasicAuthorizationHandler.BasicAuthorizationRequirement()));
// });

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json");
}

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseRouting();
// app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
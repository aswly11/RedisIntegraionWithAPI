using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RedisIntegraionWithAPI.Data;
using RedisIntegraionWithAPI.Models;
using RedisIntegraionWithAPI.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEntityFrameworkNpgsql()
.AddDbContext<AppDBContext>(opt=>opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                 ConnectionMultiplexer.Connect(new ConfigurationOptions
                 {
                     EndPoints = { "localhost:6376" },
                     AbortOnConnectFail = false,
                     Ssl = true,
                     SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                 }));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

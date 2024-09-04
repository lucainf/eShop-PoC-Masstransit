using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Products.Entities;
using Products.Models.Mapper;
using Products.Services.Masstransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IConfiguration configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ProductsContext>(options =>
{
    options.UseSqlite(configuration["ConnectionString"]);
});

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


// Add services to the container
builder.Services.AddScoped<ProductConsumer>();
builder.Services.AddScoped<CategoryConsumer>();

// Initialize MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductConsumer>();
    x.AddConsumer<CategoryConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration["RabbitMQ:Host"], h =>
        {
            h.Username(configuration["RabbitMQ:Username"]);
            h.Password(configuration["RabbitMQ:Password"]);
        });

        // Configure receive endpoints
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
using Application.Services;
using Azure.Identity;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var sqlServer = builder.Configuration["Database:Server"];
    var database = builder.Configuration["Database:Name"];

    var credential = new DefaultAzureCredential();
    var token = credential.GetToken(
        new Azure.Core.TokenRequestContext(new[] { "https://database.windows.net/" }));

    var connection = new SqlConnection($"Server={sqlServer};Database={database};")
    {
        AccessToken = token.Token
    };

    options.UseSqlServer(connection);
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

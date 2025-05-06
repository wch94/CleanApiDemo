using Application.Services;
using Azure.Identity;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://login.microsoftonline.com/5243421f-fa6a-42bb-9456-b62fc4bee840/v2.0";
        options.Audience = "a1edb4c2-a6fe-4e42-9425-73765b057d13";
    });

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
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


builder.Services.AddDbContext<AppDbContext>(options =>
{
    SqlConnection connection;

    if (bool.TryParse(builder.Configuration["LOCAL_DEVELOPMENT"], out bool isLocalDev))
    {
        var connectionString = "Data Source=my-sql-server-wch94.database.windows.net;Initial Catalog=MyDatabase;User ID=wch94_aol.com#EXT#@wch94aol.onmicrosoft.com;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Authentication=ActiveDirectoryInteractive;Application Intent=ReadWrite;Multi Subnet Failover=False";
        connection = new SqlConnection(connectionString);
    }
    else
    {
        var sqlServer = builder.Configuration["Database:Server"];
        var database = builder.Configuration["Database:Name"];

        var credential = new DefaultAzureCredential();
        var token = credential.GetToken(
            new Azure.Core.TokenRequestContext(new[] { "https://database.windows.net/" }));

        connection = new SqlConnection($"Server={sqlServer};Database={database};")
        {
            AccessToken = token.Token
        };
    }

    options.UseSqlServer(connection);
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddAutoMapper(typeof(Application.MappingProfiles.ProductProfile).Assembly);

builder.Services.AddScoped<ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

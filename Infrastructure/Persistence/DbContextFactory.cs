using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DbContextFactory
{
    public static AppDbContext Create(string sqlServer, string database)
    {
        var credential = new DefaultAzureCredential();
        var token = credential.GetToken(
            new TokenRequestContext(new[] { "https://database.windows.net/" }));

        var connection = new SqlConnection($"Server={sqlServer};Database={database};")
        {
            AccessToken = token.Token
        };

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connection)
            .Options;

        return new AppDbContext(options);
    }
}

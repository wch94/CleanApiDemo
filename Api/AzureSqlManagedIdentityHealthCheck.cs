using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApi;

public class AzureSqlManagedIdentityHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public AzureSqlManagedIdentityHealthCheck(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var credential = new DefaultAzureCredential();
            var token = await credential.GetTokenAsync(new TokenRequestContext(new[] { "https://database.windows.net/" }), cancellationToken);

            using var connection = new SqlConnection(_connectionString);
            connection.AccessToken = token.Token;

            await connection.OpenAsync(cancellationToken);
            await connection.CloseAsync();

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Azure SQL Managed Identity health check failed.", ex);
        }
    }
}

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class DesignTimeAppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Connection string only used at DESIGN TIME → put dev/test connection here
        optionsBuilder.UseSqlServer("Server=localhost;Database=MyDatabase;Trusted_Connection=True;");

        return new AppDbContext(optionsBuilder.Options);
    }
}

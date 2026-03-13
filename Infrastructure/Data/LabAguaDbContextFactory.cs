using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data
{
    /// <summary>
    /// Factory para que EF Core pueda crear el DbContext en tiempo de diseño
    /// (comandos dotnet ef migrations add / update).
    /// </summary>
    public class LabAguaDbContextFactory : IDesignTimeDbContextFactory<LabAguaDbContext>
    {
        public LabAguaDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LabAguaDbContext>();
            optionsBuilder.UseSqlite("Data Source=LabAgua_design.db");
            return new LabAguaDbContext(optionsBuilder.Options);
        }
    }
}

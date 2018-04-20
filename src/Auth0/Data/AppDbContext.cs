using Microsoft.EntityFrameworkCore;

namespace Auth0.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<RepositoryModel> Repositories { get; set; }
  }
}
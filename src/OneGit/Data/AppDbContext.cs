using Microsoft.EntityFrameworkCore;

namespace OneGit.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<RepositoryModel> Repositories { get; set; }
  }
}
using Microsoft.EntityFrameworkCore;

namespace OneGit.Api.Data
{
  public class RepositoryDbContext : DbContext
  {
    public RepositoryDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<RepositoryModel> Repositories { get; set; }
  }
}
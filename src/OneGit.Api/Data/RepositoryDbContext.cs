using Microsoft.EntityFrameworkCore;

namespace OneGit.Api.Data
{
  public class RepositoryContext : DbContext
  {
    public RepositoryContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<RepositoryModel> Repositories { get; set; }
  }
}
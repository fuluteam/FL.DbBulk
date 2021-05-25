using FL.DbBulk.Test.Entity;
using Microsoft.EntityFrameworkCore;

namespace FL.DbBulk.Test
{
    public class MyDbContext:DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {
        }

        // public DbSet<Blog> Blogs { get; set; }
        public DbSet<MiCatalog> MiCatalogs { get; set; }
    }
}
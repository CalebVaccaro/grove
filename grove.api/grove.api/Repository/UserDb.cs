using Microsoft.EntityFrameworkCore;

namespace grove.Repository;

public class UserDb : DbContext
{
    public UserDb(DbContextOptions<UserDb> options) : base(options){}
    public DbSet<User> Users => Set<User>();
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=grove.db");
    }
}
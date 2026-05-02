using Microsoft.EntityFrameworkCore;

namespace ProducerDotNet
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Isso criará uma tabela chamada "Transactions" no PostgreSQL
        public DbSet<Transaction> Transactions { get; set; }
    }
}
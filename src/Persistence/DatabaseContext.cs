using Microsoft.EntityFrameworkCore;
using src.Models;

namespace src.Persistence;

public class DatabaseContext : DbContext
{
    public DatabaseContext(
        DbContextOptions<DatabaseContext> options
    ) : base(options)
    {
        
    }
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Contrato> Contratos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {   
        builder.Entity<Pessoa>(e => {
            e.HasKey(e => e.Id);
            e.HasMany(e => e.Contratos)
                .WithOne()
                .HasForeignKey(c => c.PessoaId);
        });

        builder.Entity<Contrato>(e => {
            e.HasKey(e => e.Id);
        });
    }
}
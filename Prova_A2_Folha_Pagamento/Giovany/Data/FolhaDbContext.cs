using Giovany.Models;
using Microsoft.EntityFrameworkCore;

namespace Giovany.Data;

public class FolhaDbContext(DbContextOptions<FolhaDbContext> options) : DbContext(options)
{
    public DbSet<FolhaPagamento> Folhas => Set<FolhaPagamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var folha = modelBuilder.Entity<FolhaPagamento>();
        folha.ToTable("Folhas");
        folha.HasKey(x => x.Id);
        folha.HasIndex(x => new { x.Cpf, x.Mes, x.Ano }).IsUnique();
        folha.Property(x => x.Nome).IsRequired();
        folha.Property(x => x.Cpf).IsRequired();
    }
}

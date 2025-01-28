using Core.Entities;
using Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Infrastructure.Data;

public class CharityCaseContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<CharityCase> CharityCases { get; set; }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(CharityCaseConfig).Assembly);
  }
}

using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class CharityCaseConfig : IEntityTypeConfiguration<CharityCase>
{
  public void Configure(EntityTypeBuilder<CharityCase> builder)
  {
    builder.Property(x => x.AmountCollected).HasColumnType("decimal(18,2)");
    builder.Property(x => x.AmountRequested).HasColumnType("decimal(18,2)");
  }
}

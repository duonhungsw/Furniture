using Furniture.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Furniture.Infrastructure.Config;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
		builder.Property(c => c.Description).IsRequired();
	}
}

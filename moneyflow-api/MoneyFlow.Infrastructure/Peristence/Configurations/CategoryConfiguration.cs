using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyFlow.Domain.Categories;
using MoneyFlow.Domain.Users;

namespace MoneyFlow.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration
    : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(category => category.Id);

        builder.Property(category => category.Id)
            .ValueGeneratedOnAdd();

        builder.Property(category => category.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(category => category.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(category => category.Icon)
            .HasMaxLength(100);

        builder.HasIndex(category => new
            {
                category.UserId,
                category.Name,
                category.Type
            })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(category => category.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
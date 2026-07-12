using LibraryProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryProject.Infrastructure.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(b => b.ISBN)
                .IsUnique();

            builder.HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
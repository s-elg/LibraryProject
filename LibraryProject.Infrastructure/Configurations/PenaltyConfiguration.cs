using LibraryProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryProject.Infrastructure.Configurations
{
    public class PenaltyConfiguration : IEntityTypeConfiguration<Penalty>
    {
        public void Configure(EntityTypeBuilder<Penalty> builder)
        {
            builder.HasKey(p => p.Id);

            // UserId zorunlu -> ceza her zaman bir kullanıcıya bağlı olmalı
            builder.HasOne(p => p.User)
                .WithMany(u => u.Penalties)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Loan)
                .WithOne(l => l.Penalty)
                .HasForeignKey<Penalty>(p => p.LoanId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(p => p.SuspensionEndDate)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>()   // enum'ı DB'de string olarak sakla (Active/Completed okunaklı olsun)
                .HasMaxLength(20);
        }
    }
}
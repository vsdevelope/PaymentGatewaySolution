using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Persistence.Configurations
{
    public class MerchantKeyMappingConfiguration : IEntityTypeConfiguration<MerchantKeyMapping>
    {
        public void Configure(EntityTypeBuilder<MerchantKeyMapping> builder)
        {
            builder.Property(e => e.Id)
                .IsRequired()
                .UseIdentityColumn(1, 1);

            builder.Property(e => e.MerchantId)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(e => e.MerchantKey)
              .IsRequired()
              .HasMaxLength(100);

            builder.HasAlternateKey(e => new { e.MerchantKey })
               .HasName("PK_MerchantKeyMapping_Key");

            builder.HasKey(e => new { e.MerchantId })
                .HasName("PK_MerchantKeyMapping_MerchantId");
        }
    }
}

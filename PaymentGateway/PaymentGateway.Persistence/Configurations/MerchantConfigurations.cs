using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentGateway.Domain;

namespace PaymentGateway.Persistence.Configurations
{
    public class MerchantConfigurations : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {
            builder.Property(e => e.MerchantKeyMappingId)
                .IsRequired();
            builder.Property(e => e.TerminalId)
                .IsRequired()
                .HasMaxLength(10);
            
            builder.HasKey(e => new { e.TerminalId })
              .HasName("PK_Merchant_Terminal");
        }
    }
}

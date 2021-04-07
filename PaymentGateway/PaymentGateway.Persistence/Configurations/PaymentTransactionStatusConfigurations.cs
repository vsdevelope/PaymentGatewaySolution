using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Persistence.Configurations
{
    public class PaymentTransactionStatusConfigurations : IEntityTypeConfiguration<PaymentTransactionStatus>
    {
        public void Configure(EntityTypeBuilder<PaymentTransactionStatus> builder)
        {
            builder.Property(e => e.PaymentTransactionStatusId)
                .IsRequired()
                .UseIdentityColumn();

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(25);
            
            builder.HasKey(e => new { e.PaymentTransactionStatusId, })
              .HasName("PK_Payment_Transaction_Status");
        }
    }
}

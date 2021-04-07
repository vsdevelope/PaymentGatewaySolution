using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Persistence.Configurations
{
    public class PaymentTransactionTypeConfigurations : IEntityTypeConfiguration<PaymentTransactionType>
    {
        public void Configure(EntityTypeBuilder<PaymentTransactionType> builder)
        {
            builder.Property(e => e.PaymentTransactionTypeId)
                .IsRequired()
                .UseIdentityColumn();

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(25);

            builder.HasKey(e => new { e.PaymentTransactionTypeId, })
              .HasName("PK_Payment_Transaction_Type");
        }
    }
}

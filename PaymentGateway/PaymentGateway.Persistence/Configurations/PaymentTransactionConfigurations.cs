using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Persistence.Configurations
{
    public class PaymentTransactionConfigurations : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.Property(e => e.MerchantId)
               .IsRequired()
               .HasMaxLength(10);
            builder.Property(e => e.TerminalId)
                .IsRequired()
                .HasMaxLength(10);
            builder.Property(e => e.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.CardNumber)
                .IsRequired();
            builder.Property(e => e.CVV)
                .IsRequired();
            
            builder.Property(e => e.ExpiryDate)
                .IsRequired();
            builder.Property(e => e.DateTransactionCreated)
                .IsRequired();
    
            builder.Property(e => e.TransactionId)
                .IsRequired()
                .UseIdentityColumn(10000, 1);
        }
    }
}

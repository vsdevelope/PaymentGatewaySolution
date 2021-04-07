using System;

namespace PaymentGateway.Domain.Entities
{
    public class Transaction
    {
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
        public long TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public String CardNumber { get; set; }
        public String CVV { get; set; }
        public string ExpiryDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string PostCode { get; set; }
        public int TransactionTypeId { get; set; }
        public DateTimeOffset   DateTransactionCreated { get; set; }
        public DateTimeOffset DateTransactionUpdated { get; set; }
        public int TransactionStatusId { get; set; }
        public string StatusReason { get; set; }
        public string BankReference { get; set; }

    }
}

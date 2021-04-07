using PaymentGateway.Domain.Entities;
using System;

namespace PaymentGateway.Application.Queries.GetTransactionDetail
{
    public class TransactionDetailVm
    {
        public string TerminalId { get; set; }
        public long TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string PostCode { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }
        public DateTimeOffset DateTransactionCreated { get; set; }
        public DateTimeOffset DateTransactionUpdated { get; set; }
        public TransactionStatusEnum TransactionStatus { get; set; }
        public string BankReference { get; set; }
        public string StatusReason { get; set; }
    }
}
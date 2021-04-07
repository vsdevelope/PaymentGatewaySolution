namespace PaymentGateway.Application.Models.Acquirer
{
    public class AcquirerTransactionRequest
    {
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryDate { get; set; }
        public long TransactionId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string PostCode { get; set; }
        public int TransactionTypeId { get; set; }
    }
}

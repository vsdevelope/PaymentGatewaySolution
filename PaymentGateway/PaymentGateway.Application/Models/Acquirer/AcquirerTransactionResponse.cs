namespace PaymentGateway.Application.Models.Acquirer
{
    public class AcquirerTransactionResponse
    {
        public string BankReference { get; set; }
        public string Status { get; set; }
        public string StatusReason { get; set; }
    }
}

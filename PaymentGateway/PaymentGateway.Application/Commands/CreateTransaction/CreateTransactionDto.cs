namespace PaymentGateway.Application.Commands.CreateTransaction
{
    public class CreateTransactionDto
    {
        public long TransactionId { get; set; }
        public int TransactionStatusId { get; set; }
        public string StatusReason { get; set; }
    }
}

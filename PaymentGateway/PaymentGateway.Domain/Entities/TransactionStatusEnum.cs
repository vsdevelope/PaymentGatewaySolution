namespace PaymentGateway.Domain.Entities
{
    public enum TransactionStatusEnum
    {
        Succeeded=1,
        Failed=2,
        InProgress=3,
        Exception=4,
        Cancelled=5,
        ErrorOnAcquirer=6
    }
}

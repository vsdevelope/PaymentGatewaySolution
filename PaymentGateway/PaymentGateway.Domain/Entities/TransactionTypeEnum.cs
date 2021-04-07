namespace PaymentGateway.Domain.Entities
{
    public enum TransactionTypeEnum
    {
        CreditCardPayment=1,
        DebitCardPayment=2,
        PreAuth=3,
        Auth=4,
        Refund=5,
        Cancel=6
    }
}

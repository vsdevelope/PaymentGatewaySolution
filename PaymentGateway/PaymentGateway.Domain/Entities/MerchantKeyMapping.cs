namespace PaymentGateway.Domain.Entities
{
    public class MerchantKeyMapping
    {
        public int Id { get; set; }
        public string MerchantId { get; set; }
        public string MerchantKey { get; set; }
    }
}

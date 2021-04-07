namespace PaymentGateway.Api.FunctionalTests.Model
{
    public class LocalUser
    {
        public string MerchantId { get; set; }
        public string MerchantKey { get; set; }

        public LocalUser(LocalSettings settings, string username)
        {
            var type = typeof(LocalSettings);

           MerchantId = type.GetProperty($"{username}")?.GetValue(settings)?.ToString() ?? string.Empty;

           MerchantKey = type.GetProperty($"{username}Key")?.GetValue(settings)?.ToString() ?? string.Empty;
            
        }
    }
}
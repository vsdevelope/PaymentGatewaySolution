using Newtonsoft.Json;

namespace PaymentGateway.Api.FunctionalTests.Model
{
    public class LocalSettings
    {
        public string HostUrl { get; set; }
        public string Merchant1 { get; set; }
        public string Merchant1Key { get; set; }
        public string Merchant2 { get; set; }
        public string Merchant2Key { get; set; }

    }
}
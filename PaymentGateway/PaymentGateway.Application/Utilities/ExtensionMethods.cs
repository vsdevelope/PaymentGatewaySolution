using System.Text.RegularExpressions;

namespace PaymentGateway.Application.Utilities
{
    public static class ExtensionMethods
    {
        public static string MaskCreditCardNumber(this string creditcard)
        {
            
            return $"****-****-****-{creditcard.Substring(creditcard.Length - 4, 4)}";
        }

        public static string MaskCVV(this string cvv)
        {
            Regex pattern = new Regex("[0-9]");
            var maskedCvv=pattern.Replace(cvv, "*");
            return maskedCvv;
        }
    }
}

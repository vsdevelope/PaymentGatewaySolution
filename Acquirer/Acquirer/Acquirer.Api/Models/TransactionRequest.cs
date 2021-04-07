using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acquirer.Api.Models
{
    public class TransactionRequest
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
        public int TransactionType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acquirer.Api.Models
{
    public class TransactionResponse
    {
        public string BankReference { get; set; }
        public string Status { get; set; }
        public string StatusReason { get; set; }
    }
}

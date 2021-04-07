using MediatR;
using System;

namespace PaymentGateway.Application.Commands.CreateTransaction
{
    //Use below method to extract values from header
    //  [HttpPost]
    //  public String login([FromHeader(Name = "usuario")] string usuario, [FromHeader(Name = "pass")] string pass))
    //{
    //   return "works";       
    //}
    public class CreateTransactionCommand:IRequest<CreateTransactionCommandResponse>
    {
        public string MerchantKey { get; set; }
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string PostCode { get; set; }
        public int TransactionTypeId { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
    }
}

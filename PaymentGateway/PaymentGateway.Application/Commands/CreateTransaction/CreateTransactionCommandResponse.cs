using PaymentGateway.Application.Responses;

namespace PaymentGateway.Application.Commands.CreateTransaction
{
    public class CreateTransactionCommandResponse:BaseResponse
    {
        public CreateTransactionCommandResponse() : base()
        {

        }

        public CreateTransactionDto Transaction { get; set; }
    }
}
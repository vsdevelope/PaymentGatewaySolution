using MediatR;

namespace PaymentGateway.Application.Queries.GetTransactionDetail
{
    public class GetTransactionDetailQuery:IRequest<TransactionDetailVm>
    {
        public string MerchantKey { get; set; }
        public long TransactionId { get; set; }
    }
}

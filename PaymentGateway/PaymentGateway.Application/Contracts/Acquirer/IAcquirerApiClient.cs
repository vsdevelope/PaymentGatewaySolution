using PaymentGateway.Application.Models.Acquirer;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Contracts.Acquirer
{
    public interface IAcquirerApiClient
    {
        Task<AcquirerTransactionResponse> SendTransaction(AcquirerTransactionRequest request, CancellationToken cancellationToken = default);
    }
}

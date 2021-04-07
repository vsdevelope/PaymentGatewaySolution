using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentGateway.Application.Contracts.Acquirer;
using PaymentGateway.Application.Models.Acquirer;
using Polly;
using Polly.Retry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.ApiClients.Acquirer
{
    public class AcquirerApiClient : IAcquirerApiClient
    {
        private AcquirerSettings _acquirerSettings;
        private ILogger<AcquirerApiClient> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public AcquirerApiClient(IOptions<AcquirerSettings> acquirerSettings, ILogger<AcquirerApiClient> logger)
        {
            _acquirerSettings = acquirerSettings.Value;
            _logger = logger;

            _retryPolicy = Policy
                           .Handle<FlurlHttpException>()
                           .WaitAndRetryAsync(
                               new[]
                               {
                                   TimeSpan.FromSeconds(5),
                               });
        }

        public async Task<AcquirerTransactionResponse> SendTransaction(AcquirerTransactionRequest request, CancellationToken cancellationToken = default)
        {
            AcquirerTransactionResponse response = null;
            _logger.LogInformation($"Sending transaction to acquirer for transactionId {request.TransactionId}");
            try
            {
                //if (request.CardNumber.EndsWith("1111"))
                //{
                //    return new AcquirerTransactionResponse
                //    {
                //        Status = "Succeeded",
                //        BankReference = "1111",
                //        StatusReason = "Succeeded"
                //    };
                //}
                //else if (request.CardNumber.EndsWith("2222"))
                //{
                //    return new AcquirerTransactionResponse
                //    {
                //        Status = "Failed",
                //        BankReference = "2222",
                //        StatusReason = "InvalidCardDetails"
                //    };
                //}
                //else if (request.CardNumber.EndsWith("3333"))
                //{
                //    return new AcquirerTransactionResponse
                //    {
                //        Status = "InProgress",
                //        BankReference = "3333",
                //        StatusReason = "LinkFailure"
                //    };
                //}
                //else if (request.CardNumber.EndsWith("4444"))
                //{
                //    return new AcquirerTransactionResponse
                //    {
                //        Status = "Exception",
                //        BankReference = "4444",
                //        StatusReason = "SomethingWentWrong.TryAgain"
                //    };
                //}
                //else if (request.CardNumber.EndsWith("5555"))
                //{
                //    return new AcquirerTransactionResponse
                //    {
                //        Status = "Cancelled",
                //        BankReference = "5555",
                //        StatusReason = "CancelledTryAgain"
                //    };
                //}
                //else if (request.CardNumber.EndsWith("6666"))
                //{
                //    return null;
                //}
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    response = await _acquirerSettings.Url.WithHeader("ApiKey", _acquirerSettings.ApiKey)
                    .PostJsonAsync(request, cancellationToken)
                    .ReceiveJson<AcquirerTransactionResponse>();

                    _logger.LogInformation($"response received from acquirer for transactionId {request.TransactionId}");
                });
            }
            catch(FlurlHttpException ex)
            {
                _logger.LogError($"error response received from acquirer for transactionId {request.TransactionId},error:{ex.Message}");
                return default(AcquirerTransactionResponse);
            }

            return response;
            
        }
    }
}

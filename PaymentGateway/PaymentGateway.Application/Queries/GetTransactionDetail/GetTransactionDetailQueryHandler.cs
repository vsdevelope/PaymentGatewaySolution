using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentGateway.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Queries.GetTransactionDetail
{
    public class GetTransactionDetailQueryHandler : IRequestHandler<GetTransactionDetailQuery, TransactionDetailVm>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTransactionDetailQueryHandler> _logger;

        public GetTransactionDetailQueryHandler(ITransactionRepository transactionRepository,
                                                IMerchantRepository merchantRepository,
                                                IMapper mapper,
                                                ILogger<GetTransactionDetailQueryHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _merchantRepository = merchantRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<TransactionDetailVm> Handle(GetTransactionDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting details for transaction: {request.TransactionId}");
            var merchantId = await _merchantRepository.GetMerchantIdByKey(request.MerchantKey);
            _logger.LogInformation($"Getting details for transaction: {request.TransactionId},merchantId: {merchantId}");
            var transaction = await _transactionRepository.GetTransactionById(request.TransactionId, merchantId);

            var transactionDto = _mapper.Map<TransactionDetailVm>(transaction);

            return transactionDto;
        }
    }
}

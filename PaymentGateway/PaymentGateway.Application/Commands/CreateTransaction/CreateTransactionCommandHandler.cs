using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentGateway.Application.Contracts.Acquirer;
using PaymentGateway.Application.Contracts.Exceptions;
using PaymentGateway.Application.Contracts.Persistence;
using PaymentGateway.Application.Models.Acquirer;
using PaymentGateway.Application.Utilities;
using PaymentGateway.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Commands.CreateTransaction
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, CreateTransactionCommandResponse>
    {
        private readonly ITransactionRepository _transactionRepository;
        private ITransactionTypeRepository _transactionTypeRespository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IMapper _mapper;
        private IAcquirerApiClient _acquirerApiClient;
        private ILogger<CreateTransactionCommandHandler> _logger;
    
        public CreateTransactionCommandHandler(ITransactionRepository transactionRepository,
                                               ITransactionTypeRepository transactionTypeRespository,
                                               IMerchantRepository merchantRepository,
                                               IAcquirerApiClient acquirerApiClient,
                                               ILogger<CreateTransactionCommandHandler> logger,
                                               IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _transactionTypeRespository = transactionTypeRespository;
            _merchantRepository = merchantRepository;
            _acquirerApiClient = acquirerApiClient;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<CreateTransactionCommandResponse> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling create transaction command ");
            var createTransactionCommandResponse = new CreateTransactionCommandResponse();

            var validator = new CreateTransactionCommandValidator(_transactionTypeRespository);
            var validationResult = validator.Validate(request);

            if(validationResult.Errors.Count > 0)
            {
                createTransactionCommandResponse.Success = false;
                createTransactionCommandResponse.ValidationErrors = new List<string>();

                foreach(var error in validationResult.Errors)
                {
                    createTransactionCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                }

                _logger.LogError($"input validation failed for create transaction command");
            }

            if(createTransactionCommandResponse.Success)
            {
                _logger.LogInformation($"processing transaction command for merchantId:{request.MerchantId},terminalId:{request.TerminalId}, for amount:{request.Amount},Card Number:{request.CardNumber.MaskCreditCardNumber()}");

                //check if merchant is authorised to perform the transaction
                var merchantIdFromDb = await _merchantRepository.GetMerchantIdByKey(request.MerchantKey);

                if (merchantIdFromDb != request.MerchantId)
                {
                    var message = $"invalid merchantKey specified for merchantId:{request.MerchantId}";
                    _logger.LogError(message);
                    throw new UnAuthorizedException(message);
                }

                _logger.LogInformation($"Checking if terminal {request.TerminalId} is associated with merchant:{request.MerchantId}");

                var isTerminalAssociatedWithMerchant = await _merchantRepository.IsTerminalAssociatedWithMerchant(request.MerchantId, request.TerminalId);

                if (!isTerminalAssociatedWithMerchant)
                {
                    var message = $"terminal {request.TerminalId} is not associated with merchantId:{request.MerchantId}";
                    _logger.LogError(message);
                    throw new ForbiddenException(message);
                }
                
                var transactionToAdd = _mapper.Map<Transaction>(request);
                transactionToAdd.DateTransactionUpdated = DateTimeOffset.Now;
                var transactionAdded = await _transactionRepository.AddAsync(transactionToAdd);
                
                    var acquirerResponse = await _acquirerApiClient.SendTransaction(_mapper.Map<AcquirerTransactionRequest>(request));
                    
                    //StatusReason will aid merchant to retry the transaction, for example redirect the customer
                    //to payment page incase of invalid card details.
                    if(acquirerResponse==null)
                    {
                        transactionAdded.TransactionStatusId = (int)TransactionStatusEnum.ErrorOnAcquirer;
                        transactionAdded.StatusReason = "Error from Acquirer";
                    _logger.LogError("Error communicating with Acquirer");
                    }
                    else
                    {
                        transactionAdded.TransactionStatusId = (int)Enum.Parse<TransactionStatusEnum>(acquirerResponse.Status);
                        transactionAdded.StatusReason = acquirerResponse.StatusReason;
                        transactionAdded.BankReference = acquirerResponse.BankReference;
                    }

                transactionToAdd.DateTransactionUpdated = DateTimeOffset.Now;

                await _transactionRepository.UpdateAsync(transactionAdded);
                createTransactionCommandResponse.Transaction = _mapper.Map<CreateTransactionDto>(transactionAdded);
            }

            return createTransactionCommandResponse;
        }
    }
}

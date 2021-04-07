using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using PaymentGateway.Application.Commands.CreateTransaction;
using PaymentGateway.Application.Contracts.Acquirer;
using PaymentGateway.Application.Contracts.Exceptions;
using PaymentGateway.Application.Contracts.Persistence;
using PaymentGateway.Application.Models.Acquirer;
using PaymentGateway.Application.Profiles;
using PaymentGateway.Application.Utilities;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Tests.Common;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Application.UnitTests.Handlers
{
    [TestFixture]
    public class CreateTransactionCommandHandlerTests
    {
        private Mock<ITransactionRepository> _mockTransactionRepository;
        private Mock<ITransactionTypeRepository> _mockTransactionTypeRepository;
        private Mock<IMerchantRepository> _mockMerchantRepository;
        private Mock<IAcquirerApiClient> _mockAcquirerClient;
        private Mock<IEncryptionService> _mockEncryptionService;
        private LoggingHelper _logger;
        private IMapper _realMapper;
        private CreateTransactionCommandHandler _sut;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _mockEncryptionService = new Mock<IEncryptionService>();
          
            _realMapper= new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile(_mockEncryptionService.Object));
            }).CreateMapper();
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockTransactionTypeRepository = new Mock<ITransactionTypeRepository>();
            _mockMerchantRepository = new Mock<IMerchantRepository>();
            _mockAcquirerClient = new Mock<IAcquirerApiClient>();
          
            _logger = new LoggingHelper();
            _sut = new CreateTransactionCommandHandler(_mockTransactionRepository.Object,
                                                       _mockTransactionTypeRepository.Object,
                                                       _mockMerchantRepository.Object,
                                                       _mockAcquirerClient.Object,
                                                       _logger.CreateLogger<CreateTransactionCommandHandler>(),
                                                       _realMapper);
        }

        [Test(Description = "SCENARIO: Validation failed for transaction input")]
        public async Task GivenInvalidData_WhenTransactionIsProcessed_ThenResponseIncludesValidationErrors()
        {
            //Arrange 
            var request = new CreateTransactionCommand
            {
                MerchantId = "M0001",
                Amount = -100.12m,
                TerminalId = "T0001",
                Currency = "GBP",
                CardNumber = "51237896345612341689",
                CVV = "12345",
                ExpiryDate = "03/12",
                CustomerName = "Customer 1",
                CustomerAddressLine1 = "AL1",
                PostCode = "AB1 2CD",
                TransactionTypeId = (int)TransactionTypeEnum.CreditCardPayment,
                TransactionDate = DateTimeOffset.Now.AddSeconds(-20)
            };

            //Act
            var response = await _sut.Handle(request, default);

            //Assert
            response.Success.Should().BeFalse();
            response.ValidationErrors.Count.Should().BeGreaterThan(0);
            _logger.Information.Should().NotBeEmpty();
            _logger.Errors.Should().NotBeEmpty();
        }

        [Test(Description = "SCENARIO: Validation successful, merchant doesn't have terminal registered")]
        public void GivenValidData_WhenTerminalNotAssociatedWithMerchant_ThenThrowsForbiddenException()
        {
            //Arrange 
            var request = new CreateTransactionCommand
            {
                MerchantKey="MerchantKey",
                MerchantId = "M0001",
                Amount = 100.12m,
                TerminalId = "T0001",
                Currency = "GBP",
                CardNumber = "5123789634561234",
                CVV = "123",
                ExpiryDate = DateTime.Today.ToString("MM/yy"),
                CustomerName = "Customer 1",
                CustomerAddressLine1 = "AL1",
                PostCode = "AB1 2CD",
                TransactionTypeId = (int)TransactionTypeEnum.CreditCardPayment,
                TransactionDate = DateTimeOffset.Now.AddSeconds(-20)
            };

            _mockTransactionTypeRepository.Setup(x =>
                      x.GetByIdAsync(It.IsAny<int>()))
                        .ReturnsAsync((int t)=>new PaymentTransactionType { PaymentTransactionTypeId = t, Description = "CreditCardPayment" });

            _mockMerchantRepository.Setup(x =>
             x.GetMerchantIdByKey(request.MerchantKey)
             )
            .ReturnsAsync(request.MerchantId);

            _mockMerchantRepository.Setup(x =>
                        x.IsTerminalAssociatedWithMerchant(It.IsAny<string>(),It.IsAny<string>())
                        )
                .ReturnsAsync(false);

            //Act
            var ex = Assert.ThrowsAsync<ForbiddenException>(async () => await _sut.Handle(request, default));

            //Assert
            ex.Message.Should().Be($"terminal {request.TerminalId} is not associated with merchantId:{request.MerchantId}");
            _logger.Information.Should().NotBeNull();
            _logger.Errors.Should().NotBeNull();
        }

        [Test(Description = "SCENARIO: Validation successful, Transaction is processed successfully and Accquirer is errored out")]
        public async Task GivenValidData_WhenTransactionIsProcessed_ThenReturnsSuccessfulResponse()
        {
            //Arrange 
            var request = new CreateTransactionCommand
            {
                MerchantKey="MerchantKey",
                MerchantId = "M0001",
                Amount = 100.12m,
                TerminalId = "T0001",
                Currency = "GBP",
                CardNumber = "5123789634561234",
                CVV = "123",
                ExpiryDate = DateTime.Now.AddMonths(1).ToString("MM/yy"),
                CustomerName = "Customer 1",
                CustomerAddressLine1 = "AL1",
                PostCode = "AB1 2CD",
                TransactionTypeId = (int)TransactionTypeEnum.CreditCardPayment,
                TransactionDate = DateTimeOffset.Now.AddSeconds(-20)
            };

            var transactionId = 1000;

            AcquirerTransactionResponse acquirerResponse = null;

            _mockTransactionTypeRepository.Setup(x =>
                      x.GetByIdAsync(It.IsAny<int>()))
                        .ReturnsAsync((int t) => new PaymentTransactionType { PaymentTransactionTypeId = t, Description = "CreditCardPayment" });

            _mockTransactionRepository.Setup(x =>
              x.AddAsync(It.IsAny<Transaction>()))
            .ReturnsAsync((Transaction t) => { t.TransactionId = transactionId; t.DateTransactionUpdated = DateTimeOffset.Now; return t; });

            _mockAcquirerClient.Setup(x =>
              x.SendTransaction(It.IsAny<AcquirerTransactionRequest>(),default))
            .ReturnsAsync(acquirerResponse);

            _mockEncryptionService.Setup(x => x.Encrypt(It.IsAny<string>()))
                 .Returns((string input) =>
                 {
                     if(input.Length<=4)
                     {
                         return "uBeKFM03ScPlfo3f/iEcUA==";
                     }

                     return "QLKHKBxBaca4O9Aac0FO3wZ4SgmkmvyWo0AUN7rqM3d2RGQPqOyjAOt4g1G8eY2x";
                 });

            _mockEncryptionService.Setup(x => x.Decrypt(It.IsAny<string>()))
                .Returns((string input)=>
                {
                    var returnValue = string.Empty;
                    var v = nameof(input);
                    if(v == "cvv")
                    {
                        returnValue = request.CVV;
                    }
                    else
                    {
                        returnValue = request.CardNumber;
                    }

                    return request.CardNumber;
                });

            _mockMerchantRepository.Setup(x =>
           x.GetMerchantIdByKey(request.MerchantKey)
           )
          .ReturnsAsync(request.MerchantId);

            _mockMerchantRepository.Setup(x =>
                        x.IsTerminalAssociatedWithMerchant(It.IsAny<string>(), It.IsAny<string>())
                        )
                .ReturnsAsync(true);

            //Act
            var response = await _sut.Handle(request, default);

            //Assert
            response.Success.Should().BeTrue();
            response.Transaction.TransactionId.Should().Be(transactionId);
            response.Transaction.TransactionStatusId.Should().Be(6);
            response.Transaction.StatusReason.Should().Be("Error from Acquirer");
            _logger.Information.Should().NotBeEmpty();
            _logger.Errors.Should().NotBeEmpty();
        }

        [Test(Description = "SCENARIO: Validation successful, Transaction is processed successfully")]
        public async Task GivenValidData_WhenTransactionIsProcessedAndAcquirereNotErroredOut_ThenReturnsSuccessfulResponse()
        {
            //Arrange 
            var request = new CreateTransactionCommand
            {
                MerchantKey="MerchantKey",
                MerchantId = "M0001",
                Amount = 100.12m,
                TerminalId = "T0001",
                Currency = "GBP",
                CardNumber = "5123789634561234",
                CVV = "123",
                ExpiryDate = DateTime.Today.ToString("MM/yy"),
                CustomerName = "Customer 1",
                CustomerAddressLine1 = "AL1",
                PostCode = "AB1 2CD",
                TransactionTypeId = (int)TransactionTypeEnum.CreditCardPayment,
                TransactionDate = DateTimeOffset.Now.AddSeconds(-20)
            };

            var transactionId = 1000;

            AcquirerTransactionResponse acquirerResponse = new AcquirerTransactionResponse
            {
                BankReference="LLY001",
                Status="Failed",
                StatusReason="InvalidCardDetails"
            };

            _mockMerchantRepository.Setup(x =>
           x.GetMerchantIdByKey(request.MerchantKey)
           )
          .ReturnsAsync(request.MerchantId);

            _mockTransactionTypeRepository.Setup(x =>
                      x.GetByIdAsync(It.IsAny<int>()))
                        .ReturnsAsync((int t) => new PaymentTransactionType { PaymentTransactionTypeId = t, Description = "CreditCardPayment" });

            _mockTransactionRepository.Setup(x =>
              x.AddAsync(It.IsAny<Transaction>()))
            .ReturnsAsync((Transaction t) => { t.TransactionId = transactionId; t.DateTransactionUpdated = DateTimeOffset.Now; return t; });

            _mockAcquirerClient.Setup(x =>
              x.SendTransaction(It.IsAny<AcquirerTransactionRequest>(), default))
            .ReturnsAsync(acquirerResponse);

            _mockEncryptionService.Setup(x => x.Encrypt(It.IsAny<string>()))
                 .Returns((string input) =>
                 {
                     if (input.Length <= 4)
                     {
                         return "uBeKFM03ScPlfo3f/iEcUA==";
                     }

                     return "QLKHKBxBaca4O9Aac0FO3wZ4SgmkmvyWo0AUN7rqM3d2RGQPqOyjAOt4g1G8eY2x";
                 });

            _mockMerchantRepository.Setup(x =>
                        x.IsTerminalAssociatedWithMerchant(It.IsAny<string>(), It.IsAny<string>())
                        )
                .ReturnsAsync(true);

            //Act
            var response = await _sut.Handle(request, default);

            //Assert
            response.Success.Should().BeTrue();
            response.Transaction.TransactionId.Should().Be(transactionId);
            response.Transaction.TransactionStatusId.Should().Be(2);
            response.Transaction.StatusReason.Should().Be(acquirerResponse.StatusReason);
            _logger.Information.Should().NotBeEmpty();
            _logger.Errors.Should().BeEmpty();
        }
    }
}


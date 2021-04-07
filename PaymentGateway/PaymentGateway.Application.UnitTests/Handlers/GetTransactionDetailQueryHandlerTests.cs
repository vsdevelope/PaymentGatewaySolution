using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using PaymentGateway.Application.Contracts.Persistence;
using PaymentGateway.Application.Profiles;
using PaymentGateway.Application.Queries.GetTransactionDetail;
using PaymentGateway.Application.Utilities;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Tests.Common;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Application.UnitTests.Handlers
{
    [TestFixture]
    public class GetTransactionDetailQueryHandlerTests
    {
        private Mock<ITransactionRepository> _mockTransactionRepository;
        private Mock<IMerchantRepository> _mockMerchantRepository;
        private Mock<IEncryptionService> _mockEncryptionService;
        private LoggingHelper _logger;
        private IMapper _realMapper;
        private GetTransactionDetailQueryHandler _sut;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _mockEncryptionService = new Mock<IEncryptionService>();

            _realMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile(_mockEncryptionService.Object));
            }).CreateMapper();
        }
        [SetUp]
        public void BeforeEachTest()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockMerchantRepository = new Mock<IMerchantRepository>();

            _logger = new LoggingHelper();
            _sut = new GetTransactionDetailQueryHandler(_mockTransactionRepository.Object,
                                                       _mockMerchantRepository.Object,
                                                       _realMapper,
                                                       _logger.CreateLogger<GetTransactionDetailQueryHandler>()
                                                       );
        }

        [Test(Description = "SCENARIO: Transaction not found")]
        public async Task GivenTransactionId_WhenTransactionNotFound_ThenResponseShouldReturnEmpty()
        {

            //Arrange
            var request = new GetTransactionDetailQuery
            {
                TransactionId = 10001,
                MerchantKey = "MerchantKey"
            };

            var merchantId = "M0001";

            var transactionResponse = new Transaction
            {
                MerchantId = "M0001",
                Amount = 100.12m,
                TerminalId = "T0001",
                Currency = "GBP",
                CardNumber= "QLKHKBxBaca4O9Aac0FO3wZ4SgmkmvyWo0AUN7rqM3d2RGQPqOyjAOt4g1G8eY2x",
                CVV = "uBeKFM03ScPlfo3f/iEcUA==",
                ExpiryDate = DateTime.Today.ToString("MM/yy"),
                CustomerName = "Customer 1",
                CustomerAddressLine1 = "AL1",
                PostCode = "AB1 2CD",
                TransactionTypeId = (int)TransactionTypeEnum.CreditCardPayment,
                DateTransactionCreated = DateTimeOffset.Now.AddSeconds(-20),
                DateTransactionUpdated = DateTimeOffset.Now,
                TransactionId = request.TransactionId,
                TransactionStatusId = (int)TransactionStatusEnum.InProgress,
                BankReference = "HSBC001",
                StatusReason = "LongRunningTransaction"
            };

            _mockMerchantRepository.Setup(x =>
                     x.GetMerchantIdByKey(request.MerchantKey))
             .ReturnsAsync(merchantId);

            _mockTransactionRepository.Setup(x =>
                   x.GetTransactionById(request.TransactionId, merchantId))
           .ReturnsAsync(transactionResponse);

            _mockEncryptionService.Setup(x => x.Decrypt(It.IsAny<string>()))
                .Returns((string input) =>
                { 
                    if (input == "QLKHKBxBaca4O9Aac0FO3wZ4SgmkmvyWo0AUN7rqM3d2RGQPqOyjAOt4g1G8eY2x")
                    {
                        return "5123789634561234";
                    }
                    else
                    {
                        return "123";
                    }
                });

            //Act
            var response = await _sut.Handle(request, default);

            //Assert
            response.TransactionId.Should().Be(request.TransactionId);
        }
    }
}

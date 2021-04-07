using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using PaymentGateway.Application.Commands.CreateTransaction;
using PaymentGateway.Application.Models.Acquirer;
using PaymentGateway.Application.Profiles;
using PaymentGateway.Application.Queries.GetTransactionDetail;
using PaymentGateway.Application.Utilities;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.UnitTests.Mappings
{
    [TestFixture]
    public class PaymentGatewayMappingProfileTests
    {
        private Fixture _fixture;
        private IMapper _realMapper;
        private Mock<IEncryptionService> _mockEncryptionService;

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
            _fixture = new Fixture();
        }

        [Test(Description = "SCENARIO: Map CreateTransactionCommand To Transaction")]
        public void CreateTransactionCommand_To_Transaction()
        {
            // Arrange
            var request = _fixture.Build< CreateTransactionCommand>()
                .With(t=>t.TransactionTypeId,3)
                .Create();
            var cvvEncrypt = "uBeKFM03ScPlfo3f/iEcUA==";
            var cardEncrypt = "QLKHKBxBaca4O9Aac0FO3wZ4SgmkmvyWo0AUN7rqM3d2RGQPqOyjAOt4g1G8eY2x";

            _mockEncryptionService.Setup(x => x.Encrypt(It.IsAny<string>()))
                 .Returns((string input) =>
                 {
                     if (input.Contains("CVV"))
                     {
                         return cvvEncrypt;
                     }

                     return cardEncrypt;
                 });

            // Act
            var mappedRequest = _realMapper
                 .Map<Transaction>(request);

            mappedRequest.CardNumber.Should().Be(cardEncrypt);
            mappedRequest.Amount.Should().Be(request.Amount);
            mappedRequest.Currency.Should().Be(request.Currency);
            mappedRequest.CustomerAddressLine1.Should().Be(request.CustomerAddressLine1);
            mappedRequest.CustomerName.Should().Be(request.CustomerName);
            mappedRequest.CVV.Should().Be(cvvEncrypt);
            mappedRequest.ExpiryDate.Should().Be(request.ExpiryDate);
            mappedRequest.DateTransactionCreated.Should().Be(request.TransactionDate);
            mappedRequest.ExpiryDate.Should().Be(request.ExpiryDate);
            mappedRequest.MerchantId.Should().Be(request.MerchantId);
            mappedRequest.PostCode.Should().Be(request.PostCode);
            mappedRequest.TransactionTypeId.Should().Be(request.TransactionTypeId);
        }

        [Test(Description = "SCENARIO: Map Transaction to TransactionDetailVm")]
        public void Transaction_To_TransactionDetailVm()
        {
            // Arrange
            var request = _fixture.Build<Transaction>()
                .With(t => t.TransactionTypeId, 1)
                .With(t => t.TransactionStatusId, 2)
                .Create();

            var cvvDecrypt = "123";
            var cardDecrypt = "4444324509883400";

            _mockEncryptionService.Setup(x => x.Decrypt(It.IsAny<string>()))
                 .Returns((string input) =>
                 {
                     if (input.Contains("CVV"))
                     {
                         return cvvDecrypt;
                     }

                     return cardDecrypt;
                 });

            // Act
            var mappedRequest = _realMapper
                 .Map<TransactionDetailVm>(request);

            request.CardNumber = mappedRequest.CardNumber;
            request.CVV = mappedRequest.CVV;

            mappedRequest.TerminalId.Should().Be(request.TerminalId);
            mappedRequest.TransactionId.Should().Be(request.TransactionId);
            mappedRequest.Amount.Should().Be(request.Amount);
            mappedRequest.CardNumber.Should().Be(cardDecrypt.MaskCreditCardNumber());
            mappedRequest.Currency.Should().Be(request.Currency);
            mappedRequest.CustomerAddressLine1.Should().Be(request.CustomerAddressLine1);
            mappedRequest.CustomerName.Should().Be(request.CustomerName);
            mappedRequest.CVV.Should().Be(cvvDecrypt.MaskCVV());
            mappedRequest.ExpiryDate.Should().Be(request.ExpiryDate);
            mappedRequest.StatusReason.Should().Be(request.StatusReason);
            mappedRequest.DateTransactionCreated.Should().Be(request.DateTransactionCreated);
            mappedRequest.DateTransactionUpdated.Should().Be(request.DateTransactionUpdated);
            mappedRequest.ExpiryDate.Should().Be(request.ExpiryDate);
            mappedRequest.PostCode.Should().Be(request.PostCode);
            mappedRequest.BankReference.Should().Be(request.BankReference);
            mappedRequest.TransactionType.Should().Be(request.TransactionTypeId);
            mappedRequest.TransactionStatus.Should().Be(request.TransactionStatusId);
            mappedRequest.BankReference.Should().Be(request.BankReference);
        }

        [Test(Description = "SCENARIO: Map Transaction to CreateTransactionDto")]
        public void Transaction_To_CreateTransactionDto()
        {
            // Arrange
            var request = _fixture.Create<Transaction>();
         
            // Act
            var mappedRequest = _realMapper
                 .Map<CreateTransactionDto>(request);

            mappedRequest.StatusReason.Should().Be(request.StatusReason);
            mappedRequest.TransactionId.Should().Be(request.TransactionId);
            mappedRequest.TransactionStatusId.Should().Be(request.TransactionStatusId);
        }

        [Test(Description = "SCENARIO: Map CreateTransactionCommand to AcquirerTransactionRequest")]
        public void CreateTransactionCommand_To_AcquirerTransactionRequest()
        {
            // Arrange
            var request = _fixture.Create<CreateTransactionCommand>();

            // Act
            var mappedRequest = _realMapper
                 .Map<AcquirerTransactionRequest>(request);

            mappedRequest.CardNumber.Should().Be(request.CardNumber);
            mappedRequest.Currency.Should().Be(request.Currency);
            mappedRequest.CustomerAddressLine1.Should().Be(request.CustomerAddressLine1);
            mappedRequest.CustomerName.Should().Be(request.CustomerName);
            mappedRequest.CVV.Should().Be(request.CVV);
            mappedRequest.ExpiryDate.Should().Be(request.ExpiryDate);
            mappedRequest.MerchantId.Should().Be(request.MerchantId);
            mappedRequest.ExpiryDate.Should().Be(request.ExpiryDate);
            mappedRequest.MerchantId.Should().Be(request.MerchantId);
            mappedRequest.PostCode.Should().Be(request.PostCode);
            mappedRequest.TransactionTypeId.Should().Be(request.TransactionTypeId);
        }
    }
}

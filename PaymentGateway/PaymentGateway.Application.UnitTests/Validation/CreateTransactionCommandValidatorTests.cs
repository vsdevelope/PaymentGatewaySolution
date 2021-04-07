using FluentAssertions;
using Moq;
using NUnit.Framework;
using PaymentGateway.Application.Commands.CreateTransaction;
using PaymentGateway.Application.Contracts.Persistence;
using PaymentGateway.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Application.UnitTests.Validation
{
    [TestFixture]
    class CreateTransactionCommandValidatorTests
    {
        private CreateTransactionCommandValidator _sut;
        private Mock<ITransactionTypeRepository> _mockTransactionTypeRepository;

        [SetUp]
        public void Setup()
        {
            _mockTransactionTypeRepository = new Mock<ITransactionTypeRepository>();
            _sut = new CreateTransactionCommandValidator(_mockTransactionTypeRepository.Object);
        }

        [Test(Description = "SCENARIO: Valid input shouldn't result in error")]
        public async Task GivenValidCommand_WhenValidated_ThenShouldNotReturnValidationErrors()
        {
            //Arrange
            var request = new CreateTransactionCommand
            {
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
            _mockTransactionTypeRepository.Setup(x =>
                    x.GetByIdAsync(It.IsAny<int>()))
                      .ReturnsAsync((int t) => new PaymentTransactionType { PaymentTransactionTypeId = t, Description = "CreditCardPayment" });

            //Act
            var validationResult = await _sut.ValidateAsync(request);

            //Assert
            validationResult.Errors.Count.Should().Be(0);
        }

        [Test(Description = "SCENARIO: InValid input should result in errors")]
        public async Task GivenInvalidCommand_WhenValidated_ThenShouldReturnValidationErrors()
        {
            //Arrange
            var request = new CreateTransactionCommand
            {
                Amount = -100.12m,
                CardNumber = "1234",
                CVV = "12356",
                ExpiryDate = DateTime.Now.AddMonths(-1).ToString("MM/yy"),
                TransactionTypeId = (int)TransactionTypeEnum.DebitCardPayment,
                TransactionDate = DateTimeOffset.Now.AddHours(1)
            };
            
            _mockTransactionTypeRepository.Setup(x =>
                    x.GetByIdAsync(It.IsAny<int>()))
                      .ReturnsAsync((int t) => new PaymentTransactionType { PaymentTransactionTypeId = t, Description = "CreditCardPayment" });

            //Act
            var validationResult = await _sut.ValidateAsync(request);

            //Assert
            validationResult.Errors.Count.Should().Be(11);
        }
    }
}

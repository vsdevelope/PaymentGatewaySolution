using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Application.Commands.CreateTransaction;
using PaymentGateway.Application.Queries.GetTransactionDetail;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Tests.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Api.UnitTests
{
    [TestFixture]
    public class TransactionControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private LoggingHelper _log;
        private TransactionController _sut;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _mockMediator = new Mock<IMediator>(MockBehavior.Strict);
            _log = new LoggingHelper();
            _sut = new TransactionController(_mockMediator.Object, _log.CreateLogger<TransactionController>());
        }

        [TearDown]
        public void AfterEachTest()
        {
            _mockMediator.VerifyAll();
        }

        [Test(Description = "SCENARIO: Transaction found")]
        public async Task GivenTheTransactionIsFound_WhenTheHandlerReturnsResponse_ThenTheApiReturnsOk()
        {
            // Arrange
            var expectedTransaction =
                new TransactionDetailVm()
                {
                    Amount = 100.12m,
                    TerminalId = "T0001",
                    TransactionId = 15000,
                    Currency = "GBP",
                    CardNumber = "************1234",
                    CVV = "***",
                    ExpiryDate = "03/12",
                    CustomerName = "Customer 1",
                    CustomerAddressLine1 = "AL1",
                    PostCode = "AB1 2CD",
                    TransactionType = TransactionTypeEnum.CreditCardPayment,
                    TransactionStatus = TransactionStatusEnum.Succeeded,
                    DateTransactionCreated = DateTimeOffset.Now.AddSeconds(-20),
                    DateTransactionUpdated = DateTimeOffset.Now,
                    BankReference="HSBC001",
                    StatusReason="Succeeded"
                };
            
            _mockMediator
                .Setup(
                    x => x.Send(
                        It.IsAny<GetTransactionDetailQuery>(),
                        default))
                .ReturnsAsync(() => expectedTransaction);

            //act
            var response = await _sut.GetTransactionById(expectedTransaction.TransactionId, "merchantKey");

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            var transactionResponse = (response as OkObjectResult).Value;
            transactionResponse.Should().BeOfType<TransactionDetailVm>();
            transactionResponse.Should().Be(expectedTransaction);
            _log.Information.Should().NotBeEmpty();

        }

        [Test(Description = "SCENARIO: Transaction not found")]
        public async Task GivenTheTransactionIsNotFound_WhenTheHandlerReturnsNull_ThenTheApiReturnsNotFound()
        {
            // Arrange
            TransactionDetailVm expectedTransaction = null;

            _mockMediator
                .Setup(
                    x => x.Send(
                        It.IsAny<GetTransactionDetailQuery>(),
                        default))
                .ReturnsAsync(() => expectedTransaction);

            //act
            var response = await _sut.GetTransactionById(1, "merchantKey");

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
            var transactionResponse = (response as NotFoundObjectResult).Value;
            transactionResponse.Should().NotBeNull();
            _log.Information.Should().NotBeEmpty();
        }

        [Test(Description = "SCENARIO: Post transaction unsuccessful due to validation errors")]
        public async Task GivenThePostTransactionFails_WhenTheHandlerReturns_ThenTheApiReturnsBadRequestAndSuccessIsFalse()
        {
            // Arrange
            var createTransactionCommandResponse = new CreateTransactionCommandResponse()
            {
                Success = false,
                Message = null,
                Transaction = null,
                ValidationErrors = new List<string> { "TerminalId is required" }
            };

            _mockMediator
                .Setup(
                    x => x.Send(
                        It.IsAny<CreateTransactionCommand>(),
                        default))
                .ReturnsAsync(createTransactionCommandResponse);

            // Act
            var response = await _sut.PostTransaction(
                new CreateTransactionCommand(),
                default);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
            var value = (response as BadRequestObjectResult).Value;
            value.Should().BeOfType<CreateTransactionCommandResponse>();

            var actualResponse = value as CreateTransactionCommandResponse;
            actualResponse.ValidationErrors.Should()
                              .BeEquivalentTo(createTransactionCommandResponse.ValidationErrors);
            actualResponse.Success.Should().BeFalse();

            _log.Information.Should().NotBeEmpty();
        }

        [Test(Description = "SCENARIO: Post transaction successful")]
        public async Task GivenThePostTransactionSuccessful_WhenTheHandlerReturns_ThenTheApiReturnsOkRequestAndDisplaysTransaction()
        {
            // Arrange
            var transactionResult = new CreateTransactionDto { 
                TransactionId = 1, 
                TransactionStatusId = 1, 
                StatusReason = "Succeeded" 
            };
            var createTransactionCommandResponse = new CreateTransactionCommandResponse()
            {
                Success = true,
                Message = null,
                Transaction = transactionResult,
                ValidationErrors = null
            };

            _mockMediator
                .Setup(
                    x => x.Send(
                        It.IsAny<CreateTransactionCommand>(),
                        default))
                .ReturnsAsync(createTransactionCommandResponse);

            // Act
            var response = await _sut.PostTransaction(
                new CreateTransactionCommand(),
                default);

            // Assert
            response.Should().BeOfType<CreatedResult>();
            var value = (response as CreatedResult).Value;
            value.Should().BeOfType<CreateTransactionCommandResponse>();

            var actualResponse = value as CreateTransactionCommandResponse;
            actualResponse.ValidationErrors.Should().BeNull();
            actualResponse.Success.Should().BeTrue();
            actualResponse.Transaction.Should().Be(transactionResult);
                              

            _log.Information.Should().NotBeEmpty();
        }
    }
}
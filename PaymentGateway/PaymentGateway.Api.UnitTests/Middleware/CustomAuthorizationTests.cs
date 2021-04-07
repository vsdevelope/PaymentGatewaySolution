using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using PaymentGateway.Api.Middleware;
using PaymentGateway.Application.Contracts.Exceptions;
using PaymentGateway.Application.Contracts.Persistence;
using PaymentGateway.Tests.Common;
using System.IO;
using System.Threading.Tasks;

namespace PaymentGateway.Api.UnitTests.Middleware
{
    [TestFixture]
    public class CustomAuthorizationTests
    {
        private LoggingHelper _log;
        private CustomAuthorizationMiddleware _sut;
        private Mock<IMerchantRepository> _mockMerchantRepository;

        private RequestDelegate _next;
        private DefaultHttpContext _defaultHttpContext;


        const string expectedOutput = "Next delegate in the request pipeline invoked";

        [SetUp]
        public void BeforeEachTest()
        {
            _log = new LoggingHelper();
            _mockMerchantRepository = new Mock<IMerchantRepository>();
            _next = new RequestDelegate(innerHttpContext =>
              {
                  innerHttpContext.Response.WriteAsync(expectedOutput);
                  return Task.CompletedTask;
              });
            _sut = new CustomAuthorizationMiddleware(_next, _log.CreateLogger<CustomAuthorizationMiddleware>());
            _defaultHttpContext = new DefaultHttpContext();
            _defaultHttpContext.Response.Body = new MemoryStream();
        }

        [Test(Description = "SCENARIO: no authorization required for /health endpoint")]
        public async Task GivenTheRequestIsForHealthCheck_WhenNoAuthorizationHeaderPresent_ThenTheApiInvokesNextDelegate()
        {
            //Arrange
            _defaultHttpContext.Request.Path = "/health";

            //Act
            await _sut.Invoke(_defaultHttpContext, _mockMerchantRepository.Object);

            //Assert: if the next delegate is invoked
            _defaultHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = new StreamReader(_defaultHttpContext.Response.Body).ReadToEnd();
            expectedOutput.Should().Be(body);
            _log.Information.Should().NotBeEmpty();
        }

        [Test(Description = "SCENARIO: merchantKey header not found")]
        public void GivenTheMerchantKeyHeaderMissing_WhenNoAuthorizationHeaderPresent_ThenTheApiReturnsUnauthorized()
        {
            //Arrange
            _defaultHttpContext.Request.Path = "/transaction";

            //Act & Assert
            var ex = Assert.ThrowsAsync<UnAuthorizedException>(async () =>
            await _sut.Invoke(_defaultHttpContext, _mockMerchantRepository.Object));

            ex.Message.Should().Be("Provide 'merchantKey' in the header");
            _log.Information.Should().NotBeEmpty();
            _log.Errors.Should().NotBeEmpty();
        }

        [Test(Description = "SCENARIO: invalid merchantKey in the header")]
        public void GivenInvalidMerchantKeyInHeader_WhenAuthorizationFails_ThenTheApiReturnsUnauthorized()
        {
            //Arrange
            string merchantId = null;
            _defaultHttpContext.Request.Path = "/transaction";
            _defaultHttpContext.Request.Headers.Add("merchantKey", "invalidMerchantKey");
            _mockMerchantRepository.Setup(x => 
                        x.GetMerchantIdByKey(
                        It.IsAny<string>()))
                .ReturnsAsync(merchantId);

            //Act & Assert
            var ex = Assert.ThrowsAsync<UnAuthorizedException>(async () => await _sut.Invoke(_defaultHttpContext, _mockMerchantRepository.Object));

            ex.Message.Should().Be("Invalid merchantKey in the header");
            _log.Information.Should().NotBeEmpty();
            _log.Errors.Should().NotBeEmpty();

            _mockMerchantRepository.VerifyAll();
        }

        [Test(Description = "SCENARIO: valid merchantKey in the header")]
        public async Task GivenValidMerchantKeyInHeader_WhenAuthorizationSucceeds_ThenTheApiInvokesNextDelegate()
        {
            //Arrange
            string merchantId = "1234";
            _defaultHttpContext.Request.Path = "/transaction";
            _defaultHttpContext.Request.Headers.Add("merchantKey", "validMerchantKey");
            _mockMerchantRepository.Setup(x =>
                        x.GetMerchantIdByKey(
                        It.IsAny<string>()))
                .ReturnsAsync(merchantId);

            //Act 
            await _sut.Invoke(_defaultHttpContext, _mockMerchantRepository.Object);

            //Assert: if the next delegate is invoked
            _defaultHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = new StreamReader(_defaultHttpContext.Response.Body).ReadToEnd();
            expectedOutput.Should().Be(body);
            _log.Information.Should().NotBeEmpty();

            _mockMerchantRepository.VerifyAll();
        }
    }
}

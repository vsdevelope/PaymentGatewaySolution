using FluentAssertions;
using PaymentGateway.Api.FunctionalTests.Helpers;
using PaymentGateway.Api.FunctionalTests.Model;
using PaymentGateway.Application.Commands.CreateTransaction;
using PaymentGateway.Application.Queries.GetTransactionDetail;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace PaymentGateway.Api.FunctionalTests.Steps
{
    [Binding]
    public class TransactionsSteps
    {
        private readonly HttpClient _client;
        private readonly ScenarioHelper _scenario;
        private readonly LocalSettings _settings;

        public TransactionsSteps(HttpClient client, ScenarioHelper scenario, LocalSettings settings)
        {
            _client = client;
            _scenario = scenario;
            _settings = settings;
        }

        [When(@"I post a transaction")]
        public async Task WhenIPostATransaction(Table table)
        {
            if (table.Rows[0]["TransactionDate"].ToLower().Equals("<<pastdate>>"))
            {
                table.Rows[0]["TransactionDate"] = DateTimeOffset.Now.ToString();
            }
            var request=table.CreateInstance<CreateTransactionCommand>();

            if(request.ExpiryDate.ToLower().Equals("<<futuredate>>"))
            {
                request.ExpiryDate = DateTime.Now.AddMonths(1).ToString("MM/yy");
            }

            await _client.Post($"{_settings.HostUrl}/api/transaction", request.ToJson(), _scenario.GetDurable<string>("merchantKey"))
                 .ContinueWith(t=>_scenario.Response = t.Result.ToValueTuple());

            var transactionResponse = _scenario.GetResponseAs<CreateTransactionCommandResponse>();
            
            _scenario.SetDurable("transactionId", transactionResponse?.Transaction?.TransactionId);
            
            
        }

        [When(@"I try to get details of previous transaction")]
        public async Task WhenITryToGetDetailsOfPreviousTransaction()
        {
            var transactionId = _scenario.GetDurable<long>("transactionId");
            var merchantKey = _scenario.GetDurable<string>("merchantKey");
            await _client.Get($"{_settings.HostUrl}/api/transaction/{transactionId}", merchantKey)
                .ContinueWith(t => _scenario.Response = t.Result.ToValueTuple());
        }

        [Then(@"I should see transaction detail")]
        public void ThenIShouldSeeTransactionDetail(Table table)
        {
            var actualTransaction = _scenario.GetResponseAs<TransactionDetailVm>();
            var expectedTransaction = table.CreateInstance<TransactionDetailVm>();
            expectedTransaction.TransactionId = _scenario.GetDurable<long>("transactionId");
           
            expectedTransaction.Amount.Should().Be(actualTransaction.Amount);
           
            expectedTransaction.CardNumber.Should().Be(actualTransaction.CardNumber);
            expectedTransaction.Currency.Should().Be(actualTransaction.Currency);
            expectedTransaction.CustomerAddressLine1.Should().Be(actualTransaction.CustomerAddressLine1);
            expectedTransaction.CustomerName.Should().Be(actualTransaction.CustomerName);
            expectedTransaction.CVV.Should().Be(actualTransaction.CVV);
            expectedTransaction.PostCode.Should().Be(actualTransaction.PostCode);
            expectedTransaction.StatusReason.Should().Be(actualTransaction.StatusReason);
            expectedTransaction.TransactionId.Should().Be(actualTransaction.TransactionId);
            expectedTransaction.TransactionStatus.Should().Be(actualTransaction.TransactionStatus);
            expectedTransaction.TransactionType.Should().Be(actualTransaction.TransactionType);
        }



    }
}

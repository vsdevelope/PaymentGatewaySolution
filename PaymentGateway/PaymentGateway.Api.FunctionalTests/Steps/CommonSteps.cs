using FluentAssertions;
using PaymentGateway.Api.FunctionalTests.Helpers;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using System.Collections.Generic;
using System.Net.Http;
using PaymentGateway.Api.FunctionalTests.Model;
using PaymentGateway.Application.Commands.CreateTransaction;
using System.Diagnostics;

namespace PaymentGateway.Api.FunctionalTests.Steps
{
    [Binding]
    internal class CommonSteps
    {
        private readonly HttpClient _client;
        private readonly ScenarioHelper _scenario;
        private readonly LocalSettings _settings;

        public CommonSteps(HttpClient client , ScenarioHelper scenario, LocalSettings settings)
        {
            _client = client;
            _scenario = scenario;
            _settings = settings;
        }

        [Given("I am authenticated as '(.*)'")]
        public void Authenticate(string username)
        {
            var user = new LocalUser(_settings, username);

            _scenario.SetDurable("merchantKey", user.MerchantKey);
        }

        [Given("I am not authenticated merchant")]
        public void NotAuthenticate()
        {
            _scenario.SetDurable("merchantKey", "someMerchantKey");
        }


        [Then("the response has an HTTP status code of (.*)")]
        public void HttpStatusCode(HttpStatusCode status)
        {
            _scenario.Response.Status.Should().Be(status);
        }

        [Then("I should see the following validation errors")]
        public void ValidationErrors(Table table)
        {
            var validationErrors = _scenario.Response.Content
                .FromJson<CreateTransactionCommandResponse>()
                .ValidationErrors;
            foreach(var error in validationErrors)
            {
                Debug.WriteLine(error);
            }

                validationErrors.Should().BeEquivalentTo(table.AsStrings("Error"));
        }

        [Then("the content should be (.*)")]
        public void ExpectStringResponse(string expected)
        {
            _scenario.Response.Content
                .Should().Be(expected);
        }
    }
}
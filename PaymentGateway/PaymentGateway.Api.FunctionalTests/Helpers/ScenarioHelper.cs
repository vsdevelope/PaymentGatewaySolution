using System;
using System.Net;
using TechTalk.SpecFlow;

namespace PaymentGateway.Api.FunctionalTests.Helpers
{
    public class ScenarioHelper
    {
        private readonly ScenarioContext _context;

        public ScenarioHelper(ScenarioContext scenario)
        {
            _context = scenario;
            Exception = null;
        }

        internal string Request
        {
            get => _context.Get<string>("request");
            set => _context.Set(value, "request");
        }

        internal (string Content, HttpStatusCode Status) Response
        {
            get => _context.Get<(string Content, HttpStatusCode Status)>("response");
            set => _context.Set(value, "response");
        }

        internal void SetDurable<T>(string key, T value) => _context.Set<T>(value, key);

        internal T GetDurable<T>(string key) => _context.Get<T>(key);

        internal T GetResponseAs<T>() => Response.Content.FromJson<T>();

        internal Exception Exception
        {
            get => _context.Get<Exception>("exception");
            set => _context.Set(value, "exception");
        }
    }
}
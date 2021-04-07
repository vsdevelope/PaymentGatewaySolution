using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using PaymentGateway.Application.Contracts.Exceptions;
using PaymentGateway.Application.Contracts.Persistence;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Middleware
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomAuthorizationMiddleware> _logger;
        public CustomAuthorizationMiddleware(RequestDelegate next, ILogger<CustomAuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IMerchantRepository _merchantRepository)
        {
            _logger.LogInformation($"checking authorization for request path:{context.Request.Path.Value}");
            if (!(context.Request.Path.Value.Contains("health") 
                || context.Request.Path.Value.Contains("swagger")))
            {
                context.Request.Headers.TryGetValue("merchantKey", out StringValues merchantKey);
                if (string.IsNullOrWhiteSpace(merchantKey))
                {
                    _logger.LogError($"required authorization header missing");
                    throw new UnAuthorizedException("Provide 'merchantKey' in the header");
                }

                var merchantId = await _merchantRepository.GetMerchantIdByKey(merchantKey);
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    _logger.LogError($"invalid merchantkey in header");
                    throw new UnAuthorizedException("Invalid merchantKey in the header");
                }
            }

            await _next(context);
        }
    }
}

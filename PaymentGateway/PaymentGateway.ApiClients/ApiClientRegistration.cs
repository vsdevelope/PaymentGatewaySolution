using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.ApiClients.Acquirer;
using PaymentGateway.Application.Contracts.Acquirer;
using PaymentGateway.Application.Models.Acquirer;

namespace PaymentGateway.ApiClients
{
    public static class ApiClientRegistration
    {
        public static IServiceCollection AddApiClientServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.Configure<AcquirerSettings>(configuration.GetSection("AcquirerSettings"));
            services.AddTransient<IAcquirerApiClient, AcquirerApiClient>();

            return services;
        }
    }
}

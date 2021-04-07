using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application.Contracts.Persistence;
using PaymentGateway.Persistence.Repositories;

namespace PaymentGateway.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PaymentGatewayDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("PaymentGatewayConnectionString")));

            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionTypeRepository, TransactionTypeRepository>();

            return services;
        }
    }
}

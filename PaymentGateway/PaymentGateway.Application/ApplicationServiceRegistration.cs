using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application.Profiles;
using PaymentGateway.Application.Utilities;
using System.Reflection;

namespace PaymentGateway.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile(provider.GetService<IEncryptionService>()));
            }).CreateMapper());
            services.AddTransient<IEncryptionService, EncryptionService>();
      
            return services;
        }
    }
}

using PaymentGateway.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Contracts.Persistence
{
    public interface ITransactionTypeRepository
    {
        Task<PaymentTransactionType> GetByIdAsync(int id);
        Task<IReadOnlyList<PaymentTransactionType>> ListAllAsync();
    }
}

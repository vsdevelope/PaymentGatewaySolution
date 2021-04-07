using PaymentGateway.Domain.Entities;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Contracts.Persistence
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetTransactionById(long id,string merchantId);
        Task<Transaction> AddAsync(Transaction entity);
        Task UpdateAsync(Transaction entity);
    }
}

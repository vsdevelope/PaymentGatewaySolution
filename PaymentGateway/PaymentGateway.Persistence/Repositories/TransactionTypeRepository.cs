using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentGateway.Application.Contracts.Persistence;
using PaymentGateway.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Persistence.Repositories
{
    public class TransactionTypeRepository: BaseRepository<PaymentTransactionType>,ITransactionTypeRepository
    {
        private ILogger<TransactionTypeRepository> _logger;
        public TransactionTypeRepository(PaymentGatewayDbContext dbContext, ILogger<TransactionTypeRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }


        public async Task<PaymentTransactionType> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting PaymentTransactionType for id:{id}");
            var entity=await _dbContext.TransactionTypes.FindAsync(id);
            return entity;
        }

        public async Task<IReadOnlyList<PaymentTransactionType>> ListAllAsync()
        {
            _logger.LogInformation($"Getting all PaymentTransactionTypes");
            var entities = await _dbContext.TransactionTypes.ToListAsync();

            return entities;
        }
    }
}

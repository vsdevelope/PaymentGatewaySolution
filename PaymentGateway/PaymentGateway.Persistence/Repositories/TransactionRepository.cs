using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentGateway.Application.Contracts.Persistence;
using PaymentGateway.Domain.Entities;
using System.Threading.Tasks;

namespace PaymentGateway.Persistence.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>,ITransactionRepository
    {
        private ILogger<TransactionRepository> _logger;
        public TransactionRepository(PaymentGatewayDbContext dbContext, ILogger<TransactionRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }
        public TransactionRepository(PaymentGatewayDbContext dbContext) : base(dbContext)
        {
        
        }
        public async Task<Transaction> AddAsync(Transaction entity)
        {
            _logger.LogInformation($"Adding transaction for merchantId:{entity.TransactionId}, terminalId:{entity.TerminalId} for amount: {entity.Amount}");
            var entityEntry=await _dbContext.Transactions.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"transaction added successfully for merchantId:{entity.TransactionId}, terminalId:{entity.TerminalId} for amount: {entity.Amount},transactionId:{entity.TransactionId}");

            return entityEntry.Entity;
        }

        public async Task UpdateAsync(Transaction entity)
        {
             _logger.LogInformation($"Updating transaction for merchantId:{entity.TransactionId}, terminalId:{entity.TerminalId} for amount: {entity.Amount},transactionId:{entity.TransactionId}");
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"transaction updated successfully for merchantId:{entity.TransactionId}, terminalId:{entity.TerminalId} for amount: {entity.Amount},transactionId:{entity.TransactionId}");
        }

        public async Task<Transaction> GetTransactionById(long id, string merchantId)
        {
            _logger.LogInformation($"Getting transaction details for merchantId{merchantId}, transactionId:{id}");
            var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(e => e.TransactionId == id && e.MerchantId == merchantId);

            return transaction;
        }
    }
}

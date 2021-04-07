using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentGateway.Application.Contracts.Persistence;
using PaymentGateway.Domain;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Persistence.Repositories
{
    public class MerchantRepository : BaseRepository<Merchant>,IMerchantRepository
    {
        private ILogger<MerchantRepository> _logger;
        public MerchantRepository(PaymentGatewayDbContext dbContext, ILogger<MerchantRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }
   
        public async Task<string> GetMerchantIdByKey(string merchantKey)
        {
            _logger.LogInformation($"Getting merchantId by merchantKey");
            var merchantEntity = (await (_dbContext.MerchantKeyMappings.Where(x => x.MerchantKey == merchantKey)).ToListAsync()).FirstOrDefault();

            return merchantEntity?.MerchantId;
        }

        public async Task<bool> IsTerminalAssociatedWithMerchant(string merchantId, string terminalId)
        {
            _logger.LogInformation($"checking if terminal {terminalId} associated with {merchantId}");
            var merchantKeyMappingId = await _dbContext.MerchantKeyMappings.FirstOrDefaultAsync(e => e.MerchantId == merchantId);

            var actualTerminalId = await _dbContext.Merchants.AnyAsync(x => x.MerchantKeyMappingId == merchantKeyMappingId.Id && x.TerminalId == terminalId);

            return actualTerminalId;
            
        }
    }
}

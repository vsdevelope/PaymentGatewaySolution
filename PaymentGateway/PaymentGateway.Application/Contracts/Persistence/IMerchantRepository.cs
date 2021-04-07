using System.Threading.Tasks;

namespace PaymentGateway.Application.Contracts.Persistence
{
    public interface IMerchantRepository
    {
        Task<string> GetMerchantIdByKey(string merchantKey);

        Task<bool> IsTerminalAssociatedWithMerchant(string merchantId, string terminalId);
    }
}

namespace PaymentGateway.Persistence.Repositories
{
    public class BaseRepository<T> where T:class
    {
        protected readonly PaymentGatewayDbContext _dbContext;
        public BaseRepository(PaymentGatewayDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}

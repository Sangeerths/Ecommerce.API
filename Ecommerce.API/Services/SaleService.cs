using Ecommerce.API.Data;

namespace Ecommerce.API.Services
{
    public interface ISaleService
    {
    }   
    public class SaleService : ISaleService
    {
        private readonly EcommerceDbContext _dbContext;

        public SaleService(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}

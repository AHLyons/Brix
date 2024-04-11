namespace Brix.Models
{
    public class EFLegostoreRepository : ILegoStoreRepository
    {
        private BrixDatabaseContext _context;

        public EFLegostoreRepository(BrixDatabaseContext temp) 
        { 
            _context = temp;
        }

        public IQueryable<Product> Products => _context.Products;
        public IQueryable<Order> Orders => _context.Orders;
        public IQueryable<FraudPrediction> FraudPredictions => _context.FraudPredictions;
    }
}
namespace Brix.Models
{
    public class EFLegostoreRepository : ILegoStoreRepository
    {
        private IntexbrixContext _context;

        public EFLegostoreRepository(IntexbrixContext temp) 
        { 
            _context = temp;
        }

        public IQueryable<Product> Products => _context.Products;
    }
}
namespace Brix.Models
{
    public interface ILegoStoreRepository
    {
        public IQueryable<Product> Products { get; }
    }
}

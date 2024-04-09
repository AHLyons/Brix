namespace Brix.Models
{
    public interface ILegoStoreRepository
    {
        public IQueryable<Product> Legos { get; }
    }
}

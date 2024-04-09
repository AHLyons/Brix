namespace Brix.Models
{
    public interface ILegoStoreRepository
    {
        public IQueryable<Lego> Legos { get; }
    }
}

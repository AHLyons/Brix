namespace Brix.Models
{
    public interface ILegoStoreRepository
    {
        public IQueryable<Product> Products { get; }
        public IQueryable<Order> Orders { get; }
        public IQueryable<FraudPrediction> FraudPredictions { get; }
    }
}

using System.Linq;

namespace Brix.Models
{
    public interface ILegostoreRepository
    {
        public IQueryable<Product> Products { get; }
        public IQueryable<Order> Orders { get; }
        public IQueryable<FraudPrediction> FraudPredictions { get; }
        IQueryable<Product> Products { get; }

        // Method to add a new product
        Task NewProduct(Product product);

        // Method to update an existing product
        void UpdateProduct(Product product);

        // Method to delete a product
        void DeleteProduct(Product product);
    }
}

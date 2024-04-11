using System.Linq;

namespace Brix.Models
{
    public interface ILegostoreRepository
    {
        IQueryable<Product> Products { get; }

        // Method to add a new product
        Task NewProduct(Product product);

        // Method to update an existing product
        void UpdateProduct(Product product);

        // Method to delete a product
        void DeleteProduct(Product product);
    }
}

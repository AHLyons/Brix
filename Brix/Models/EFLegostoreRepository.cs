using Microsoft.EntityFrameworkCore; // Make sure you have this using directive for EF Core functionality
using System.Linq;

namespace Brix.Models
{
    public class EFLegostoreRepository : ILegostoreRepository
    {
        private BrixDatabaseContext _context;

        public EFLegostoreRepository(BrixDatabaseContext temp)
        {
            _context = temp;
        }

        public IQueryable<Product> Products => _context.Products;

        // Add a new product to the database
        public async Task NewProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }


        // Update an existing product in the database
        public void UpdateProduct(Product product)
        {
            _context.Products.Attach(product);
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // Delete a product from the database
        public void DeleteProduct(Product product)
        {
            if (_context.Entry(product).State == EntityState.Detached)
            {
                _context.Products.Attach(product);
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}

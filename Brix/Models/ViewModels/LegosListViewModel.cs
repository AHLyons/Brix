// In LegoListViewModel.cs
using System.Collections.Generic;

namespace Brix.Models.ViewModels;

public class LegosListViewModel
{
    public IQueryable<Product> Products { get; set; }
    public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
    public string CurrentCategory { get; set; }
    public string CurrentColor { get; set; }
    public int PageSize { get; set; }
    // Add these lists
    public List<string> Categories { get; set; } = new List<string>();
    public List<string> PrimaryColors { get; set; } = new List<string>();
}

public class ProductDetailsViewModel
{
    public Product Product { get; set; }
    public List<Product> RecommendedProducts { get; set; } = new List<Product>();
}

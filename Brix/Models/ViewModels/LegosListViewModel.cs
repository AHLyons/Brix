namespace Brix.Models.ViewModels
{
    public class LegosListViewModel
    {
        public IQueryable<Product> Products { get; set;}

        public PaginationInfo PaginationInfo { get; set;} = new PaginationInfo();
    }
}
namespace Brix.Models.ViewModels
{
    public class LegosListViewModel
    {
        public IQueryable<Product> Legos { get; set;}

        public PaginationInfo PaginationInfo { get; set;} = new PaginationInfo();
    }
}
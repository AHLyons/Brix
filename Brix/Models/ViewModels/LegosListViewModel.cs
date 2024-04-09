namespace Brix.Models.ViewModels
{
    public class LegosListViewModel
    {
        public IQueryable<Lego> Legos { get; set;}

        public PaginationInfo PaginationInfo { get; set;} = new PaginationInfo();
    }
}
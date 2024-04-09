namespace Brix.Models
{
    public interface ILegostoreRepository
    {
        public IQueryable<Lego> Legos { get; }
    }
}

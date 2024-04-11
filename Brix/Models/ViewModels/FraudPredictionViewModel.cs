namespace Brix.Models.ViewModels
{
    public class FraudPredictionViewModel
    {
        public IQueryable<Order> Orders { get; set; }
    }
}

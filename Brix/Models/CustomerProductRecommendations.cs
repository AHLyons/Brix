using System.ComponentModel.DataAnnotations;

namespace Brix.Models
{
    public class CustomerProductRecommendations
    {
        [Key]
        public int? CustomerId { get; set; }
        public string? RecommendedProduct { get; set; }
        public int? EstCustomerID { get; set; }
    }
}

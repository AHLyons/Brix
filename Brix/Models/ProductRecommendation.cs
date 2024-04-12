using System.ComponentModel.DataAnnotations;

namespace Brix.Models
{
    public partial class ProductRecommendation
    {
        [Key]
        public int? ProductId { get; set; }
        public int? RecommendedProductId { get; set; }
        public float? SimilarityScore { get; set; }
    }
}

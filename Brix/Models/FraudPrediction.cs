using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Brix.Models;
public partial class FraudPrediction
{
    [Key]
    public int? PredictionId { get; set; }
    public Order Orders { get; set; }
    public string Prediction { get; set; }
}

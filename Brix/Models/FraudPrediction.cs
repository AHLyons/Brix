using System;
using System.Collections.Generic;

namespace Brix.Models;
    public partial class FraudPrediction
    {
        public Order Fraud { get; set; }
        public string Prediction { get; set; }
    }
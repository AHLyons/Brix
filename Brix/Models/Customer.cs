using System;
using System.Collections.Generic;

namespace Brix.Models;

public partial class Customer
{
    public int? CustomerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? BirthDate { get; set; }

    public string? CountryOfResidence { get; set; }

    public string? Gender { get; set; }

    public int? Age { get; set; }
}

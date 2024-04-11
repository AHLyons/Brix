using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Brix.Models;

public partial class Customer
{
    [Key]
    public int? CustomerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? BirthDate { get; set; }

    public string? CountryOfResidence { get; set; }

    public string? Gender { get; set; }

    public int? Age { get; set; }
}

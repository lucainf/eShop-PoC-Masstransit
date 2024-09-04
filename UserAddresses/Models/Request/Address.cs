using System.ComponentModel.DataAnnotations;

namespace UserAddresses.Models.Request;

public class Address
{   
    [Required]
    [MaxLength(100)]
    public string Street { get; set; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; }

    [Required]
    [MaxLength(20)]
    public string Postcode { get; set; }

    [Required]
    [MaxLength(50)]
    public string Country { get; set; }
    
}
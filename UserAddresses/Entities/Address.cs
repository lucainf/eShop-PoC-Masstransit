using System.ComponentModel.DataAnnotations;

namespace UserAddresses.Entities;

public class Address
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; }

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
    
    public bool Deleted { get; set; }
}
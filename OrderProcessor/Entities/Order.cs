using System.ComponentModel.DataAnnotations;

namespace OrderProcessor.Entities;

public class Order
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public DateTime OrderDate { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    [Required]
    public int Address { get; set; }
    
    public decimal Total { get; set; }
    
    public ICollection<int> ProductIds { get; set; } = new List<int>();
}
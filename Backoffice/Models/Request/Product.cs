using System.ComponentModel.DataAnnotations;

namespace Backoffice.Models.Request;

public class Product
{
    [MaxLength(100)]
    public string? Name { get; set; }
    
    public decimal? Price { get; set; }
    
    public string? Description { get; set; }
    
    public int? CategoryId { get; set; }
}
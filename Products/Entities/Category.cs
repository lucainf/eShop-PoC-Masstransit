using System.ComponentModel.DataAnnotations;

namespace Products.Entities;

public class Category
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    // Navigation property
    public IEnumerable<Product> Products { get; set; }
}
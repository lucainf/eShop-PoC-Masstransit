using System.ComponentModel.DataAnnotations;

namespace Products.Entities;

public class Product
{
        [Key]
        public int Id { get; set; }
    
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    
        [Required]
        public decimal Price { get; set; }
    
        public string Description { get; set; }
    
        public int CategoryId { get; set; }
    
        // Navigation property
        public Category Category { get; set; }
}
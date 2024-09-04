using System.ComponentModel.DataAnnotations;

namespace Backoffice.Models.Request;

public class Category
{
    [MaxLength(100)]
    public string? Name { get; set; }
    
    public string? Description { get; set; }
}
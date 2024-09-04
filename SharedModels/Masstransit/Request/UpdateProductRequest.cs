namespace SharedModels.Masstransit.Request;

public class UpdateProductRequest
{
    public int ProductId { get; set; }
    
    // Inherit all properties from Product model
    public string? Name { get; set; }
    
    public decimal? Price { get; set; }
    
    public string? Description { get; set; }
    
    public int? CategoryId { get; set; }
}
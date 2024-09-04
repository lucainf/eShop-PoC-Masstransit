namespace SharedModels.Masstransit.Request;

public class UpdateCategoryRequest
{
    public int CategoryId { get; set; }
    
    // properties from Category model
    
    public string? Name { get; set; }
    
    public string? Description { get; set; }
}
namespace SharedModels.Masstransit.Request;

public class DeleteCategoryRequest
{
    public int CategoryId { get; set; }
    public int? NewCategoryId { get; set; }
}
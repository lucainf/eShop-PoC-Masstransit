namespace OrderProcessor.Models.Dto;

public class Order
{
    public int Id { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public int Address { get; set; }
    
    public ICollection<int> ProductIds { get; set; } = new List<int>();
    
    public decimal Total { get; set; }
}
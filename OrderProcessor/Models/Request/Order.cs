namespace OrderProcessor.Models.Request;

public class Order
{
    public ICollection<int> Products { get; set; }= new List<int>();
    public int AddressId { get; set; }
}
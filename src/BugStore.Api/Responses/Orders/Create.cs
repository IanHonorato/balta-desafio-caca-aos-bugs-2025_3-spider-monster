namespace BugStore.Responses.Orders;

public class Create
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderLinesResponse> Lines { get; set; } = new();
}


public class OrderLinesResponse
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
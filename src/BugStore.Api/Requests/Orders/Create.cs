using BugStore.Models;

namespace BugStore.Requests.Orders;

public class Create
{
    public Guid CustomerId { get; set; }
    public List<OrderLineCreate> Lines { get; set; } = new();
}


public class OrderLineCreate
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
}
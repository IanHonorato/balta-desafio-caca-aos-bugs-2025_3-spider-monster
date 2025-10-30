namespace BugStore.Responses.Customers;

public class Get
{
    public IEnumerable<GetById> Customers { get; set; } = new List<GetById>();
}
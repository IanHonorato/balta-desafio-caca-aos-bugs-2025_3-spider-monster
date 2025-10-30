namespace BugStore.Responses.Products;

public class Get
{
    public IEnumerable<GetById> Products { get; set; } = new List<GetById>();
}
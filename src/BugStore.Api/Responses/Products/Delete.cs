namespace BugStore.Responses.Products;

public class Delete
{
    public Guid Id { get; set; }
    public bool Deleted { get; set; }
}
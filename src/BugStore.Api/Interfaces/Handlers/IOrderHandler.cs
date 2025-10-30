namespace BugStore.Interfaces.Handlers
{
    public interface IOrderHandler
    {
        Task<Responses.Orders.Create> HandleCreate(Requests.Orders.Create request);
        Task<Responses.Orders.GetById?> HandleGetById(Requests.Orders.GetById request);
    }
}

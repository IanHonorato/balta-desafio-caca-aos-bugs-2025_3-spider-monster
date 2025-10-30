namespace BugStore.Interfaces.Handlers
{
    public interface ICustomerHandler
    {
        Task<Responses.Customers.Create> HandleCreate(Requests.Customers.Create request);
        Task<Responses.Customers.Update?> HandleUpdate(Requests.Customers.Update request);
        Task<Responses.Customers.Delete?> HandleDelete(Requests.Customers.Delete request);
        Task<Responses.Customers.GetById?> HandleGetById(Requests.Customers.GetById request);
        Task<Responses.Customers.Get> HandleGetAll();
    }
}

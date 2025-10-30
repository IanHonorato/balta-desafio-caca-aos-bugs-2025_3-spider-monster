namespace BugStore.Interfaces.Handlers
{
    public interface IProductHandler
    {
        Task<Responses.Products.Create> HandleCreate(Requests.Products.Create request);
        Task<Responses.Products.Update?> HandleUpdate(Requests.Products.Update request);
        Task<Responses.Products.Delete?> HandleDelete(Requests.Products.Delete request);
        Task<Responses.Products.GetById?> HandleGetById(Requests.Products.GetById request);
        Task<Responses.Products.Get> HandleGetAll();
    }
}

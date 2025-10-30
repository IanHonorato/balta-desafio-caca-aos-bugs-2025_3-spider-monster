using BugStore.Data;
using BugStore.Handlers;
using BugStore.Handlers.Customers;
using BugStore.Handlers.Orders;
using BugStore.Interfaces.Handlers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var cnnString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlite(cnnString));

builder.Services.AddScoped<ICustomerHandler, CustomerHandler>();
builder.Services.AddScoped<IProductHandler, ProductHandler>();
builder.Services.AddScoped<IOrderHandler, OrderHandler>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/v1/customers", async (ICustomerHandler handler) =>
{
    var response = await handler.HandleGetAll();
    return Results.Ok(response);
});

app.MapGet("/v1/customers/{id:guid}", async (Guid id, ICustomerHandler handler) =>
{
    var response = await handler.HandleGetById(new BugStore.Requests.Customers.GetById { Id = id });
    return response is null ? Results.NotFound() : Results.Ok(response);
});

app.MapPost("/v1/customers", async (BugStore.Requests.Customers.Create request, ICustomerHandler handler) =>
{
    var response = await handler.HandleCreate(request);
    return Results.Created($"/v1/customers/{response.Id}", response);
});

app.MapPut("/v1/customers/{id:guid}", async (Guid id, BugStore.Requests.Customers.Update request, ICustomerHandler handler) =>
{
    if (id != request.Id) return Results.BadRequest();
    var response = await handler.HandleUpdate(request);
    return response is null ? Results.NotFound() : Results.Ok(response);
});

app.MapDelete("/v1/customers/{id:guid}", async (Guid id, ICustomerHandler handler) =>
{
    var response = await handler.HandleDelete(new BugStore.Requests.Customers.Delete { Id = id });
    return response is null ? Results.NotFound() : Results.Ok(response);
});

app.MapGet("/v1/products", async (IProductHandler handler) =>
{
    var response = await handler.HandleGetAll();
    return Results.Ok(response);
});

app.MapGet("/v1/products/{id:guid}", async (Guid id, IProductHandler handler) =>
{
    var response = await handler.HandleGetById(new BugStore.Requests.Products.GetById { Id = id });
    return response is null ? Results.NotFound() : Results.Ok(response);
});

app.MapPost("/v1/products", async (BugStore.Requests.Products.Create request, IProductHandler handler) =>
{
    var response = await handler.HandleCreate(request);
    return Results.Created($"/v1/products/{response.Id}", response);
});

app.MapPut("/v1/products/{id:guid}", async (Guid id, BugStore.Requests.Products.Update request, IProductHandler handler) =>
{
    if (id != request.Id) return Results.BadRequest();
    var response = await handler.HandleUpdate(request);
    return response is null ? Results.NotFound() : Results.Ok(response);
});

app.MapDelete("/v1/products/{id:guid}", async (Guid id, IProductHandler handler) =>
{
    var response = await handler.HandleDelete(new BugStore.Requests.Products.Delete { Id = id });
    return response is null ? Results.NotFound() : Results.Ok(response);
});

app.MapGet("/v1/orders/{id:guid}", async (Guid id, IOrderHandler handler) =>
{
    var response = await handler.HandleGetById(new BugStore.Requests.Orders.GetById { Id = id });
    return response is null ? Results.NotFound() : Results.Ok(response);
});

app.MapPost("/v1/orders", async (BugStore.Requests.Orders.Create request, IOrderHandler handler) =>
{
    var response = await handler.HandleCreate(request);
    return Results.Created($"/v1/orders/{response.Id}", response);
});

app.Run();

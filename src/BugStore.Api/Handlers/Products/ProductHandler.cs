using BugStore.Data;
using BugStore.Interfaces.Handlers;
using BugStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Handlers;

public class ProductHandler : IProductHandler
{
    private readonly AppDbContext _context;

    public ProductHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Responses.Products.Create> HandleCreate(Requests.Products.Create request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Slug = request.Slug,
            Price = request.Price
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new Responses.Products.Create
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Slug = product.Slug,
            Price = product.Price
        };
    }

    public async Task<Responses.Products.Update?> HandleUpdate(Requests.Products.Update request)
    {
        var product = await _context.Products.FindAsync(request.Id);
        if (product == null) return null;

        product.Title = request.Title;
        product.Description = request.Description;
        product.Slug = request.Slug;
        product.Price = request.Price;

        await _context.SaveChangesAsync();

        return new Responses.Products.Update
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Slug = product.Slug,
            Price = product.Price
        };
    }

    public async Task<Responses.Products.Delete?> HandleDelete(Requests.Products.Delete request)
    {
        var product = await _context.Products.FindAsync(request.Id);
        if (product == null) return null;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return new Responses.Products.Delete
        {
            Id = request.Id,
            Deleted = true
        };
    }

    public async Task<Responses.Products.GetById?> HandleGetById(Requests.Products.GetById request)
    {
        var product = await _context.Products.FindAsync(request.Id);
        if (product == null) return null;

        return new Responses.Products.GetById
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Slug = product.Slug,
            Price = product.Price
        };
    }

    public async Task<Responses.Products.Get> HandleGetAll()
    {
        var products = await _context.Products.ToListAsync();
        return new Responses.Products.Get
        {
            Products = products.Select(p => new Responses.Products.GetById
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Slug = p.Slug,
                Price = p.Price
            }).ToList()
        };
    }
}

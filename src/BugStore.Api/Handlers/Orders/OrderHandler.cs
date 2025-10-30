using BugStore.Data;
using BugStore.Interfaces.Handlers;
using BugStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Handlers.Orders
{
    public class OrderHandler : IOrderHandler
    {
        private readonly AppDbContext _context;

        public OrderHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Responses.Orders.Create> HandleCreate(Requests.Orders.Create request)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Lines = new List<OrderLine>()
            };

            foreach (var line in request.Lines)
            {
                var product = await _context.Products.FindAsync(line.ProductId);
                if (product == null) continue; // ou lançar exceção se quiser validar

                order.Lines.Add(new OrderLine
                {
                    Id = Guid.NewGuid(),
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    Total = product.Price * line.Quantity
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new Responses.Orders.Create
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                Lines = order.Lines.Select(l => new Responses.Orders.OrderLinesResponse
                {
                    ProductId = l.ProductId,
                    Quantity = l.Quantity
                }).ToList()
            };
        }

       

        public async Task<Responses.Orders.GetById?> HandleGetById(Requests.Orders.GetById request)
        {
            var order = await _context.Orders
                .Include(o => o.Lines)
                .FirstOrDefaultAsync(o => o.Id == request.Id);

            if (order == null) return null;

            return new Responses.Orders.GetById
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Lines = order.Lines.Select(l => new Responses.Orders.OrderLinesResponse
                {
                    ProductId = l.ProductId,
                    Quantity = l.Quantity
                }).ToList()
            };
        }
    }
}

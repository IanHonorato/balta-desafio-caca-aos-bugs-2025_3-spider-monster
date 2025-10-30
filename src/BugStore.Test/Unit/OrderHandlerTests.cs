using BugStore.Data;
using BugStore.Handlers.Orders;
using BugStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Tests.Handlers
{
    public class OrderHandlerTests
    {
        private readonly AppDbContext _context;
        private readonly OrderHandler _handler;

        public OrderHandlerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _handler = new OrderHandler(_context);
        }

        [Fact]
        public async Task HandleCreate_ShouldCreateOrderWithLines()
        {
            // Arrange
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Email = "john@test.com",
                Phone = "999999999",
                BirthDate = new DateTime(1990, 1, 1)
            };
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = "Product 1",
                Description = "Test product",
                Slug = "product-1",
                Price = 50m
            };

            _context.Customers.Add(customer);
            _context.Products.Add(product);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var request = new Requests.Orders.Create
            {
                CustomerId = customer.Id,
                Lines = new List<Requests.Orders.OrderLineCreate>
                {
                    new Requests.Orders.OrderLineCreate
                    {
                        ProductId = product.Id,
                        Quantity = 2
                    }
                }
            };

            // Act
            var result = await _handler.HandleCreate(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customer.Id, result.CustomerId);
            Assert.Single(result.Lines);

            var createdOrder = await _context.Orders
                .Include(o => o.Lines)
                .FirstOrDefaultAsync(o => o.Id == result.Id, TestContext.Current.CancellationToken);

            Assert.NotNull(createdOrder);
            Assert.Single(createdOrder!.Lines);
            Assert.Equal(product.Price * 2, createdOrder.Lines.First().Total);
        }

        [Fact]
        public async Task HandleCreate_ShouldSkipInvalidProducts()
        {
            // Arrange
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "Jane Doe",
                Email = "jane@test.com",
                Phone = "888888888",
                BirthDate = new DateTime(1995, 5, 5)
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var request = new Requests.Orders.Create
            {
                CustomerId = customer.Id,
                Lines = new List<Requests.Orders.OrderLineCreate>
                {
                    new Requests.Orders.OrderLineCreate
                    {
                        ProductId = Guid.NewGuid(), // inexistente
                        Quantity = 1
                    }
                }
            };

            // Act
            var result = await _handler.HandleCreate(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Lines);
        }

        [Fact]
        public async Task HandleGetById_ShouldReturnExistingOrder()
        {
            // Arrange
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "Order Test",
                Email = "order@test.com",
                Phone = "7777777",
                BirthDate = new DateTime(1988, 8, 8)
            };

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = "Laptop",
                Description = "Powerful device",
                Slug = "laptop",
                Price = 1000m
            };

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Lines = new List<OrderLine>
                {
                    new OrderLine
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Quantity = 1,
                        Total = 1000m
                    }
                }
            };

            _context.Customers.Add(customer);
            _context.Products.Add(product);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var request = new Requests.Orders.GetById { Id = order.Id };

            // Act
            var result = await _handler.HandleGetById(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result!.Id);
            Assert.Equal(customer.Id, result.CustomerId);
            Assert.Single(result.Lines);
        }

        [Fact]
        public async Task HandleGetById_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var request = new Requests.Orders.GetById { Id = Guid.NewGuid() };

            // Act
            var result = await _handler.HandleGetById(request);

            // Assert
            Assert.Null(result);
        }
    }
}

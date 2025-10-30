using BugStore.Data;
using BugStore.Handlers;
using BugStore.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BugStore.Tests.Handlers
{
    public class ProductHandlerTests
    {
        private readonly AppDbContext _context;
        private readonly ProductHandler _handler;

        public ProductHandlerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _handler = new ProductHandler(_context);
        }

        [Fact]
        public async Task HandleCreate_ShouldAddProduct()
        {
            // Arrange
            var request = new Requests.Products.Create
            {
                Title = "Product A",
                Description = "Description A",
                Slug = "product-a",
                Price = 100m
            };

            // Act
            var result = await _handler.HandleCreate(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.Price, result.Price);

            var createdProduct = await _context.Products.FindAsync([result.Id], TestContext.Current.CancellationToken);
            Assert.NotNull(createdProduct);
            Assert.Equal(request.Slug, createdProduct!.Slug);
        }

        [Fact]
        public async Task HandleUpdate_ShouldModifyExistingProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = "Old Title",
                Description = "Old Description",
                Slug = "old-slug",
                Price = 50m
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var request = new Requests.Products.Update
            {
                Id = product.Id,
                Title = "New Title",
                Description = "New Description",
                Slug = "new-slug",
                Price = 150m
            };

            // Act
            var result = await _handler.HandleUpdate(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Title", result!.Title);
            Assert.Equal(150m, result.Price);

            var updatedProduct = await _context.Products.FindAsync([product.Id], TestContext.Current.CancellationToken);
            Assert.Equal("new-slug", updatedProduct!.Slug);
        }

        [Fact]
        public async Task HandleDelete_ShouldRemoveProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = "To Delete",
                Description = "Delete Desc",
                Slug = "delete-slug",
                Price = 30m
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var request = new Requests.Products.Delete { Id = product.Id };

            // Act
            var result = await _handler.HandleDelete(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result!.Deleted);
            Assert.Empty(_context.Products);
        }

        [Fact]
        public async Task HandleGetById_ShouldReturnExistingProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = "Product X",
                Description = "Desc X",
                Slug = "product-x",
                Price = 75m
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var request = new Requests.Products.GetById { Id = product.Id };

            // Act
            var result = await _handler.HandleGetById(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result!.Id);
            Assert.Equal("Product X", result.Title);
        }

        [Fact]
        public async Task HandleGetById_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var request = new Requests.Products.GetById { Id = Guid.NewGuid() };

            // Act
            var result = await _handler.HandleGetById(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task HandleGetAll_ShouldReturnAllProducts()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { Id = Guid.NewGuid(), Title = "A", Description = "Desc A", Slug = "a", Price = 10m },
                new Product { Id = Guid.NewGuid(), Title = "B", Description = "Desc B", Slug = "b", Price = 20m }
            );
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            // Act
            var result = await _handler.HandleGetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Products.Count()); // ou Assert.Equal(2, result.Products!.Count)
        }
    }
}

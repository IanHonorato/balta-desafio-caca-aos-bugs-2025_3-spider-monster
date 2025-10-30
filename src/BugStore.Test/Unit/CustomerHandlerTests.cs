using BugStore.Data;
using BugStore.Handlers.Customers;
using BugStore.Models;
using BugStore.Requests.Customers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BugStore.Tests.Handlers;

public class CustomerHandlerTests
{
    private readonly AppDbContext _context;
    private readonly CustomerHandler _handler;

    public CustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // banco isolado por teste
            .Options;

        _context = new AppDbContext(options);
        _handler = new CustomerHandler(_context);
    }

    [Fact]
    public async Task HandleCreate_ShouldAddCustomer()
    {
        // Arrange
        var request = new Create
        {
            Name = "Ian Honorato",
            Email = "ian@test.com",
            Phone = "999999999",
            BirthDate = new DateTime(1990, 1, 1)
        };

        // Act
        var result = await _handler.HandleCreate(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Single(_context.Customers);
    }

    [Fact]
    public async Task HandleUpdate_ShouldModifyExistingCustomer()
    {
        // Arrange
        var existing = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Email = "old@test.com",
            Phone = "1111111",
            BirthDate = new DateTime(1980, 1, 1)
        };

        _context.Customers.Add(existing);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new Update
        {
            Id = existing.Id,
            Name = "New Name",
            Email = "new@test.com",
            Phone = "2222222",
            BirthDate = new DateTime(1995, 5, 5)
        };

        // Act
        var result = await _handler.HandleUpdate(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);

        var updated = await _context.Customers.FindAsync([existing.Id], TestContext.Current.CancellationToken);
        Assert.Equal("new@test.com", updated?.Email);
    }

    [Fact]
    public async Task HandleUpdate_ShouldReturnNull_WhenCustomerNotFound()
    {
        // Arrange
        var request = new Update
        {
            Id = Guid.NewGuid(),
            Name = "Missing",
            Email = "missing@test.com",
            Phone = "000000",
            BirthDate = DateTime.Now
        };

        // Act
        var result = await _handler.HandleUpdate(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleDelete_ShouldRemoveCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "To Delete",
            Email = "delete@test.com",
            Phone = "333333",
            BirthDate = DateTime.Now
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new Delete { Id = customer.Id };

        // Act
        var result = await _handler.HandleDelete(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Deleted);
        Assert.Empty(_context.Customers);
    }

    [Fact]
    public async Task HandleDelete_ShouldReturnDeletedFalse_WhenCustomerNotFound()
    {
        // Arrange
        var request = new Delete { Id = Guid.NewGuid() };

        // Act
        var result = await _handler.HandleDelete(request);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Deleted);
    }

    [Fact]
    public async Task HandleGetById_ShouldReturnCustomer_WhenExists()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Get Test",
            Email = "get@test.com",
            Phone = "555555",
            BirthDate = new DateTime(1990, 1, 1)
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new GetById { Id = customer.Id };

        // Act
        var result = await _handler.HandleGetById(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Name, result.Name);
    }

    [Fact]
    public async Task HandleGetById_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var request = new GetById { Id = Guid.NewGuid() };

        // Act
        var result = await _handler.HandleGetById(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleGetAll_ShouldReturnAllCustomers()
    {
        // Arrange
        _context.Customers.AddRange(
            new Customer
            {
                Id = Guid.NewGuid(),
                Name = "A",
                Email = "a@test.com",
                Phone = "111111111", 
                BirthDate = new DateTime(1990, 1, 1) 
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                Name = "B",
                Email = "b@test.com",
                Phone = "222222222",
                BirthDate = new DateTime(1992, 2, 2)
            }
        );

        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await _handler.HandleGetAll();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Customers);
        Assert.Equal(2, result.Customers!.Count());
    }
}

using Bogus;
using EcommerceModular.Application.Interfaces.Messaging;
using EcommerceModular.Application.Interfaces.Persistence;
using EcommerceModular.Application.Interfaces.Repositories;
using EcommerceModular.Application.Orders.Commands.CreateOrder;
using EcommerceModular.Application.Strategies.OrderTotal;
using EcommerceModular.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Orders.Application.Tests;

[TestFixture]
public class CreateOrderCommandHandlerTests
{
    private CreateOrderCommandHandler _handler;
    private IOrderRepository _orderRepo;
    private OrderTotalStrategySelector _strategySelector;
    private IEventProducer _eventProducer;
    private IOrderReadProjection _projection;

    [SetUp]
    public void Setup()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();

        // Register the real NormalStrategy — this avoids cast issues
        serviceProvider
            .GetService(typeof(NormalStrategy))
            .Returns(new NormalStrategy());

        _orderRepo = Substitute.For<IOrderRepository>();
        _eventProducer = Substitute.For<IEventProducer>();
        _projection = Substitute.For<IOrderReadProjection>();
        _strategySelector = new OrderTotalStrategySelector(serviceProvider);

        _handler = new CreateOrderCommandHandler(
            _orderRepo,
            _strategySelector,
            _eventProducer,
            _projection
        );
    }

    [Test]
    public async Task Handle_Should_Create_Order_And_Return_Id()
    {
        // Arrange
        var faker = new Faker();
        var command = new CreateOrderCommand
        {
            CustomerId = faker.Random.Guid().ToString(),
            CustomerType = "Normal",
            ShippingAddress = new()
            {
                Street = faker.Address.StreetName(),
                City = faker.Address.City(),
                State = faker.Address.State(),
                Country = faker.Address.Country(),
                ZipCode = faker.Address.ZipCode()
            },
            Items = new()
            {
                new() { ProductId = Guid.NewGuid(), ProductName = "Test", Quantity = 1, UnitPrice = 10.0m }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        await _orderRepo.Received(1).AddAsync(Arg.Any<Order>());
        await _eventProducer.Received(1).ProduceAsync("orders.created", Arg.Any<object>());
        await _projection.Received(1).ProjectAsync(Arg.Any<Order>());
    }

    [Test]
    public async Task Handle_Should_Throw_When_CustomerType_Is_Invalid()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            CustomerId = Guid.NewGuid().ToString(),
            CustomerType = "Unknown", // invalid
            ShippingAddress = new(),
            Items = new()
        };

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid customer type: Unknown");
    }

    [Test]
    public async Task Handle_Should_Create_Order_With_Multiple_Items()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            CustomerId = Guid.NewGuid().ToString(),
            CustomerType = "Normal",
            ShippingAddress = new(),
            Items = new()
            {
                new() { ProductId = Guid.NewGuid(), ProductName = "Product A", Quantity = 2, UnitPrice = 15.0m },
                new() { ProductId = Guid.NewGuid(), ProductName = "Product B", Quantity = 1, UnitPrice = 20.0m },
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        await _orderRepo.Received(1).AddAsync(Arg.Any<Order>());
        await _eventProducer.Received(1).ProduceAsync("orders.created", Arg.Any<object>());
        await _projection.Received(1).ProjectAsync(Arg.Any<Order>());
    }

    [Test]
    public async Task Handle_Should_Work_With_Empty_Items()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            CustomerId = Guid.NewGuid().ToString(),
            CustomerType = "Normal",
            ShippingAddress = new(),
            Items = new() // empty list
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        await _orderRepo.Received(1).AddAsync(Arg.Any<Order>());
        await _eventProducer.Received(1).ProduceAsync("orders.created", Arg.Any<object>());
        await _projection.Received(1).ProjectAsync(Arg.Any<Order>());
    }
}
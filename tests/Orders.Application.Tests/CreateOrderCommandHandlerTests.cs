using Bogus;
using EcommerceModular.Application.Common.Metrics;
using EcommerceModular.Application.Interfaces.Messaging;
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
    private IOrderMetrics _orderMetrics;

    [SetUp]
    public void Setup()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider
            .GetService(typeof(NormalStrategy))
            .Returns(new NormalStrategy());

        _orderRepo = Substitute.For<IOrderRepository>();
        _eventProducer = Substitute.For<IEventProducer>();
        _strategySelector = new OrderTotalStrategySelector(serviceProvider);
        _orderMetrics = Substitute.For<IOrderMetrics>();

        _handler = new CreateOrderCommandHandler(
            _orderRepo,
            _strategySelector,
            _eventProducer,
            _orderMetrics
        );
    }

    [Test]
    public async Task Handle_Should_Create_Order_And_Return_Id()
    {
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

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        await _orderRepo.Received(1).AddAsync(Arg.Any<Order>());
        await _eventProducer.Received(1).ProduceAsync("orders.created", Arg.Any<object>());
    }

    [Test]
    public async Task Handle_Should_Throw_When_CustomerType_Is_Invalid()
    {
        var command = new CreateOrderCommand
        {
            CustomerId = Guid.NewGuid().ToString(),
            CustomerType = "Unknown",
            ShippingAddress = new(),
            Items = new()
        };

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid customer type: Unknown");
    }

    [Test]
    public async Task Handle_Should_Work_With_Multiple_Items()
    {
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

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        await _orderRepo.Received(1).AddAsync(Arg.Any<Order>());
        await _eventProducer.Received(1).ProduceAsync("orders.created", Arg.Any<object>());
    }

    [Test]
    public async Task Handle_Should_Work_With_Empty_Items()
    {
        var command = new CreateOrderCommand
        {
            CustomerId = Guid.NewGuid().ToString(),
            CustomerType = "Normal",
            ShippingAddress = new(),
            Items = new()
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        await _orderRepo.Received(1).AddAsync(Arg.Any<Order>());
        await _eventProducer.Received(1).ProduceAsync("orders.created", Arg.Any<object>());
    }

    [Test]
    public async Task Handle_Should_Track_Metrics_Correctly()
    {
        var command = new CreateOrderCommand
        {
            CustomerId = Guid.NewGuid().ToString(),
            CustomerType = "Normal",
            ShippingAddress = new()
            {
                Street = "Rua 1",
                City = "Cidade",
                State = "Estado",
                Country = "País",
                ZipCode = "00000-000"
            },
            Items = new()
            {
                new() { ProductId = Guid.NewGuid(), ProductName = "Produto", Quantity = 1, UnitPrice = 10.0m }
            }
        };

        var fakeTimer = Substitute.For<IDisposable>();
        _orderMetrics.MeasureOrderProcessingDuration().Returns(fakeTimer);

        await _handler.Handle(command, CancellationToken.None);

        _orderMetrics.Received(1).IncrementOrdersCreated();
        _orderMetrics.Received(1).MeasureOrderProcessingDuration();
        fakeTimer.Received(1).Dispose();
    }
}
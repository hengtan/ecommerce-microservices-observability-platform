using EcommerceModular.Application.DTOs;
using EcommerceModular.Domain.Entities;
using EcommerceModular.Application.Orders.Projections;
using EcommerceModular.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using IOrderReadProjection = EcommerceModular.Application.Interfaces.Persistence.IOrderReadProjection;

namespace EcommerceModular.Infrastructure.Projections;

public class OrderReadProjection : IOrderReadProjection
{
    private readonly IMongoCollection<OrderReadModel> _collection;

    public OrderReadProjection(IOptions<MongoSettings> mongoSettings)
    {
        var settings = mongoSettings.Value;
        Console.WriteLine($"[MongoSettings] ConnectionString: {settings.ConnectionString}");
        Console.WriteLine($"[MongoSettings] DatabaseName: {settings.Database}");
        Console.WriteLine($"[MongoSettings] Collection: {settings.Collection}");

        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.Database);
        _collection = database.GetCollection<OrderReadModel>(settings.Collection);
    }

    public async Task ProjectAsync(Order order, CancellationToken cancellationToken = default)
    {
        try
        {
            Console.WriteLine($"[Mongo] Inserting Order: {order.Id}");

            var projected = new OrderReadModel
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                ShippingAddress = new AddressDto
                {
                    Street = order.ShippingAddress.Street,
                    City = order.ShippingAddress.City,
                    State = order.ShippingAddress.State,
                    Country = order.ShippingAddress.Country,
                    ZipCode = order.ShippingAddress.ZipCode
                },
                Items = order.Items.Select(item => new OrderItemReadModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity
                }).ToList()
            };

            await _collection.ReplaceOneAsync(
                filter: Builders<OrderReadModel>.Filter.Eq(o => o.Id, order.Id),
                replacement: projected,
                options: new ReplaceOptions { IsUpsert = true },
                cancellationToken: cancellationToken
            );

            Console.WriteLine($"[Mongo] Order {order.Id} inserted/updated successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Mongo insert failed: {ex.Message}");
        }
    }

    public async Task<OrderReadModel?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var filter = Builders<OrderReadModel>.Filter.Eq(o => o.Id, orderId);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}
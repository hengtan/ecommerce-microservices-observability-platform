using System.Text.Json;
using EcommerceModular.Application.Interfaces.Persistence;
using EcommerceModular.Domain.Entities;
using MongoDB.Driver;

namespace EcommerceModular.Infrastructure.Projections;

public class OrderReadProjection : IOrderReadProjection
{
    private readonly IMongoCollection<Order> _collection;
    private const string _backupPath = "OrderBackups";

    public OrderReadProjection(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("orders_read");
        _collection = database.GetCollection<Order>("orders");

        if (!Directory.Exists(_backupPath))
        {
            Directory.CreateDirectory(_backupPath);
        }
    }

    public async Task ProjectAsync(Order order, CancellationToken cancellationToken = default)
    {
        // Save to MongoDB
        await _collection.InsertOneAsync(order, cancellationToken: cancellationToken);

        // Also write to .txt file (backup)
        var fileName = Path.Combine(_backupPath, $"order_{order.Id}.txt");
        var content = JsonSerializer.Serialize(order, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(fileName, content, cancellationToken);
    }
}
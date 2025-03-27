namespace Orders.Domain.Entities;

public class OrderItem(Guid productId, string productName, int quantity, decimal unitPrice)
{
    public Guid ProductId { get; private set; } = productId;
    public string ProductName { get; private set; } = productName;
    public int Quantity { get; private set; } = quantity;
    public decimal UnitPrice { get; private set; } = unitPrice;

    public decimal Total => Quantity * UnitPrice;
}
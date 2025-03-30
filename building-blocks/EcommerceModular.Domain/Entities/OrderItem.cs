namespace EcommerceModular.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; } // 🔑 Chave primária exigida pelo EF

    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal Total => Quantity * UnitPrice;

    public OrderItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid(); // gerar nova chave
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    // 🔒 Construtor vazio para o EF Core
    private OrderItem() { }
}
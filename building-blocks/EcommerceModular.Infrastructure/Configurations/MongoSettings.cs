namespace EcommerceModular.Infrastructure.Configurations;

public class MongoSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Collection { get; set; } = "orders"; // Adiciona isso!
}
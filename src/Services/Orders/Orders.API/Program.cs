using EcommerceModular.Application;
using EcommerceModular.Application.Common.Metrics;
using EcommerceModular.Application.Interfaces.Messaging;
using EcommerceModular.Application.Interfaces.ReadModels;
using EcommerceModular.Infrastructure;
using EcommerceModular.Infrastructure.Messaging;
using EcommerceModular.Infrastructure.Persistence;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Orders.API.Monitoring;
using Prometheus;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// ✅ Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.File("logs/orders-api.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ✅ Application e Infrastructure
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// ✅ MongoDB Client
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration["MongoSettings:ConnectionString"];
    return new MongoClient(connectionString);
});

// ✅ Registra o serializador de GUID
if (!BsonClassMap.IsClassMapRegistered(typeof(Guid)))
{
    BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
}

// ✅ Leitura de pedidos e Kafka producer
builder.Services.AddScoped<IOrderReadService, OrderReadService>();
builder.Services.AddScoped<IEventProducer, KafkaEventProducer>();

// ✅ Métricas com Prometheus
builder.Services.AddSingleton<IOrderMetrics, PrometheusOrderMetrics>();

// ✅ Controllers + Swagger + ReDoc
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.EnableAnnotations());

// ✅ Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgresql",
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "db", "sql" }
    )
    .AddMongoDb(
        sp => new MongoClient(builder.Configuration["MongoSettings:ConnectionString"]),
        name: "mongodb",
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "db", "mongo" }
    )
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis")!,
        name: "redis",
        timeout: TimeSpan.FromSeconds(5),
        tags: ["cache", "redis"]
    );

var app = builder.Build();

// ✅ Aplica migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.Migrate();
}

// ✅ Prometheus
app.UseHttpMetrics();
app.MapMetrics(); // /metrics

// ✅ Swagger + ReDoc
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders.API v1");
    c.RoutePrefix = "swagger";
});
app.UseReDoc(c =>
{
    c.SpecUrl = "/swagger/v1/swagger.json";
    c.RoutePrefix = "docs";
    c.DocumentTitle = "Orders.API - API Reference";
});

// ✅ Middlewares
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSerilogRequestLogging();
app.MapControllers();

// ✅ Health endpoints
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
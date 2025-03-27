using EcommerceModular.Application;
using EcommerceModular.Application.Common.Metrics;
using EcommerceModular.Infrastructure;
using Orders.API.Monitoring;
using Prometheus;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.File("logs/orders-api.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Register services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSingleton<IOrderMetrics, PrometheusOrderMetrics>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// (Optional) Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Middleware: Metrics
app.UseHttpMetrics();           // Track HTTP requests
app.MapMetrics();               // /metrics endpoint

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders.API v1");
    c.RoutePrefix = "swagger"; // /swagger
});

// ReDoc UI
app.UseReDoc(c =>
{
    c.SpecUrl = "/swagger/v1/swagger.json";
    c.RoutePrefix = "docs"; // /docs
    c.DocumentTitle = "Orders.API - API Reference";
});

// Middleware pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSerilogRequestLogging(); // Log HTTP requests
app.MapControllers();

// Health endpoints (optional)
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();
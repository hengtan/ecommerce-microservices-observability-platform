using EcommerceModular.Application;
using EcommerceModular.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add application & infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Optional: Add Health Checks (if you want later)
// builder.Services.AddHealthChecks();

var app = builder.Build();

// // Configure middleware
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders.API v1");
//         c.RoutePrefix = "swagger"; // Access at /swagger
//     });
//
//     app.UseReDoc(c =>
//     {
//         c.SpecUrl = "/swagger/v1/swagger.json";
//         c.RoutePrefix = "docs"; // Access at /docs
//         c.DocumentTitle = "Orders.API - API Reference";
//     });
// }
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
});

// Optional: app.UseSerilogRequestLogging(); (if you use Serilog)

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Optional: Map health endpoints
// app.MapHealthChecks("/health/live");
// app.MapHealthChecks("/health/ready");

app.Run();
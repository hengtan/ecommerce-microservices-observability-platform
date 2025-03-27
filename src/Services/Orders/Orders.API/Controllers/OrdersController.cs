using EcommerceModular.Application.Orders.Commands.CreateOrder;
using EcommerceModular.Application.Orders.Projections;
using EcommerceModular.Application.Orders.Queries.GetOrderById;
using EcommerceModular.Domain.Models.ReadModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Orders.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var orderId = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = orderId }, orderId);
    }
    
    /// <summary>
    /// Get an order by ID
    /// </summary>
    /// <param name="id">Order GUID</param>
    /// <returns>Returns full order details</returns>
    /// <response code="200">Success</response>
    /// <response code="404">Order not found</response>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Get order by ID",
        Description = "Returns order details for the given ID using cache fallback (Redis → MongoDB)"
    )]
    [SwaggerResponse(200, "Order found", typeof(OrderReadModel))]
    [SwaggerResponse(404, "Order not found")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await mediator.Send(query);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
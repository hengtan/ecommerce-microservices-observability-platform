using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Orders.Commands.CreateOrder;

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

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        return Ok(new { message = $"(Mock) Returning order with ID {id}" });
    }
}
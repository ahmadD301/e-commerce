using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;

    public OrdersController(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll()
    {
        var orders = await _orderRepository.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Order>> GetById(Guid id)
    {
        var order = await _orderRepository.GetOrderByIdAsync(new OrderId(id));
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpGet("by-customer/{customerId:guid}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetByCustomer(Guid customerId)
    {
        var orders = await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
        return Ok(orders);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var order = await _orderRepository.GetOrderByIdAsync(new OrderId(id));
        if (order == null)
        {
            return NotFound();
        }

        try
        {
            order.Cancel();
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
        {
            return BadRequest(ex.Message);
        }

        await _orderRepository.UpdateOrderAsync(order);
        return NoContent();
    }
}

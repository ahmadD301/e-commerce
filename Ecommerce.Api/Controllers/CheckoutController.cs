using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly CheckoutService _checkoutService;

    public CheckoutController(CheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpPost]
    public async Task<ActionResult<CheckoutResponse>> Checkout(CheckoutRequest request)
    {
        try
        {
            var orderId = await _checkoutService.CheckoutAsync(new CustomerId(request.CustomerId));
            return Ok(new CheckoutResponse(orderId.Value));
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
        {
            return BadRequest(ex.Message);
        }
    }
}

public record CheckoutRequest(Guid CustomerId);
public record CheckoutResponse(Guid OrderId);

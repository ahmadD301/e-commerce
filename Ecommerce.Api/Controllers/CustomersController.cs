using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomersController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
    {
        var customers = await _customerRepository.GetAllCustomersAsync();
        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Customer>> GetById(Guid id)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(new CustomerId(id));
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Create(CreateCustomerRequest request)
    {
        var customer = Customer.Create(request.Name, request.Email);
        await _customerRepository.AddCustomerAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id.Value }, customer);
    }
}

public record CreateCustomerRequest(string Name, string Email);

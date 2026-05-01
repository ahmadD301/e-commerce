using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _productRepository.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Product>> GetById(Guid id)
    {
        var product = await _productRepository.GetProductByIdAsync(new ProductId(id));
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(CreateProductRequest request)
    {
        var product = Product.Create(
            request.Name,
            new Money(request.PriceAmount, request.Currency),
            request.InitialStock);

        await _productRepository.AddProductAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id.Value }, product);
    }

    [HttpPatch("{id:guid}/stock")]
    public async Task<IActionResult> UpdateStock(Guid id, UpdateProductStockRequest request)
    {
        var product = await _productRepository.GetProductByIdAsync(new ProductId(id));
        if (product == null)
        {
            return NotFound();
        }

        if (request.Quantity == 0)
        {
            return BadRequest("Quantity must be non-zero.");
        }

        try
        {
            if (request.Quantity > 0)
            {
                product.IncreaseStock(request.Quantity);
            }
            else
            {
                product.DecreaseStock(new Quantity(Math.Abs(request.Quantity)));
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
        {
            return BadRequest(ex.Message);
        }

        await _productRepository.UpdateProductAsync(product);
        return NoContent();
    }

    [HttpPatch("{id:guid}/active")]
    public async Task<IActionResult> UpdateActive(Guid id, UpdateProductActiveRequest request)
    {
        var product = await _productRepository.GetProductByIdAsync(new ProductId(id));
        if (product == null)
        {
            return NotFound();
        }

        if (request.IsActive)
        {
            product.Activate();
        }
        else
        {
            product.Deactivate();
        }

        await _productRepository.UpdateProductAsync(product);
        return NoContent();
    }
}

public record CreateProductRequest(string Name, decimal PriceAmount, string Currency, int InitialStock);
public record UpdateProductStockRequest(int Quantity);
public record UpdateProductActiveRequest(bool IsActive);

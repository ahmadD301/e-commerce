using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IConfiguration _configuration;

    public AuthController(ICustomerRepository customerRepository, IConfiguration configuration)
    {
        _customerRepository = customerRepository;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var customers = await _customerRepository.GetAllCustomersAsync();
        var customer = customers.FirstOrDefault(c => c.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
        if (customer == null)
        {
            return Unauthorized();
        }

        var jwtSection = _configuration.GetSection("Jwt");
        var key = jwtSection["Key"] ?? string.Empty;
        var issuer = jwtSection["Issuer"] ?? string.Empty;
        var audience = jwtSection["Audience"] ?? string.Empty;
        var expiresMinutes = jwtSection["ExpiresMinutes"] ?? "60";

        if (string.IsNullOrWhiteSpace(key))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "JWT key is not configured.");
        }

        if (!int.TryParse(expiresMinutes, out var expires))
        {
            expires = 60;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.Value.ToString()),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Name, customer.Name)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expires);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new LoginResponse(tokenValue, expiresAt, customer.Id.Value));
    }
}

public record LoginRequest(string Email);
public record LoginResponse(string Token, DateTime ExpiresAt, Guid CustomerId);

using System;
using System.Threading.Tasks;
using Moq;
using Xunit;


public class CheckoutServiceTest
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock = new();
    private readonly CheckoutService _checkoutService;

    public CheckoutServiceTest()
    {
        _checkoutService = new CheckoutService(
            _customerRepositoryMock.Object,
            _productRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _paymentRepositoryMock.Object
        );
    }

    private (Customer customer, Product product) CreateCustomerWithProduct(int stock = 100)
    {
        var customerId = new CustomerId(Guid.NewGuid());
        var customer = new Customer(customerId, "John Doe", "email@domain.com");
        var product = Product.Create("Test Product", new Money(10, "USD"), stock);
        customer.Cart.AddItem(product, new Quantity(1));
        return (customer, product);
    }

    private void SetupCustomerRepositoryMock(Customer customer)
    {
        _customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(customer.Id))
            .ReturnsAsync(customer);
    }

    private void SetupProductRepositoryMock(Product product)
    {
        _productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(product.Id))
            .ReturnsAsync(product);
    }

    private void SetupOrderRepositoryMock()
    {
        _orderRepositoryMock
            .Setup(repo => repo.AddOrderAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupPaymentRepositoryMock()
    {
        _paymentRepositoryMock
            .Setup(repo => repo.AddPaymentAsync(It.IsAny<Payment>()))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task CheckoutAsync_ShouldPass_WhenAllInputsAreValid()
    {
        var (customer, product) = CreateCustomerWithProduct();
        SetupCustomerRepositoryMock(customer);
        SetupProductRepositoryMock(product);
        SetupOrderRepositoryMock();
        SetupPaymentRepositoryMock();

        var orderId = await _checkoutService.CheckoutAsync(customer.Id);

        Assert.NotNull(orderId);
    }

    [Fact]
    public async Task CheckoutAsync_ShouldFail_WhenCustomerIsInvalid()
    {
        var invalidCustomerId = new CustomerId(Guid.NewGuid());
        _customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(invalidCustomerId))
            .ReturnsAsync((Customer)null);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _checkoutService.CheckoutAsync(invalidCustomerId));

        Assert.Equal("Customer not found.", ex.Message);
    }

    [Fact]
    public async Task CheckoutAsync_ShouldFail_WhenCustomerAccountIsInactive()
    {
        var inactiveCustomerId = new CustomerId(Guid.NewGuid());
        var inactiveCustomer = new Customer(inactiveCustomerId, "John Doe", "Email@domain.com");
        inactiveCustomer.Deactivate();
        SetupCustomerRepositoryMock(inactiveCustomer);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _checkoutService.CheckoutAsync(inactiveCustomerId));

        Assert.Equal("Customer account is inactive.", ex.Message);
    }

    [Fact]
    public async Task CheckoutAsync_ShouldFail_WhenShippingCartIsEmpty()
    {
        var customerId = new CustomerId(Guid.NewGuid());
        var customer = new Customer(customerId, "John Doe", "email@domain.com");
        SetupCustomerRepositoryMock(customer);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _checkoutService.CheckoutAsync(customerId));

        Assert.Equal("Shopping cart is empty.", ex.Message);
    }

    [Fact]
    public async Task CheckoutAsync_ShouldFail_WhenProductIsInvalid()
    {
        var (customer, product) = CreateCustomerWithProduct();
        SetupCustomerRepositoryMock(customer);
        // product repo returns null
        _productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(product.Id))
            .ReturnsAsync((Product)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _checkoutService.CheckoutAsync(customer.Id));

        Assert.Equal($"Product with ID {product.Id} not found.", ex.Message);
    }

    [Fact]
    public async Task CheckoutAsync_ShouldFail_WhenProductIsOutOfStock()
    {
        var (customer, product) = CreateCustomerWithProduct(stock: 0);
        SetupCustomerRepositoryMock(customer);
        SetupProductRepositoryMock(product);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _checkoutService.CheckoutAsync(customer.Id));

        Assert.Equal($"Insufficient stock for product {product.Name}.", ex.Message);
    }

    [Fact]
    public async Task CheckoutAsync_ShouldFail_WhenPaymentFails()
    {
        var (customer, product) = CreateCustomerWithProduct();
        SetupCustomerRepositoryMock(customer);
        SetupProductRepositoryMock(product);
        SetupOrderRepositoryMock();
        _paymentRepositoryMock
            .Setup(repo => repo.AddPaymentAsync(It.IsAny<Payment>()))
            .ThrowsAsync(new InvalidOperationException("Payment failed."));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _checkoutService.CheckoutAsync(customer.Id));

        Assert.Equal("Payment failed.", ex.Message);
    }

    [Fact]
    public async Task CheckoutAsync_ShouldReduceProductStock_WhenCheckoutIsSuccessful()
    {
        var (customer, product) = CreateCustomerWithProduct(stock: 100);
        SetupCustomerRepositoryMock(customer);
        SetupProductRepositoryMock(product);
        SetupOrderRepositoryMock();
        SetupPaymentRepositoryMock();

        await _checkoutService.CheckoutAsync(customer.Id);

        Assert.Equal(99, product.Stock);
    }

    [Fact]
    public async Task CheckoutAsync_ShouldClearCart_AfterSuccessfulCheckout()
    {
        var (customer, product) = CreateCustomerWithProduct();
        SetupCustomerRepositoryMock(customer);
        SetupProductRepositoryMock(product);
        SetupOrderRepositoryMock();
        SetupPaymentRepositoryMock();

        await _checkoutService.CheckoutAsync(customer.Id);

        Assert.Empty(customer.Cart.Items);
    }

    [Fact]
    public async Task CheckoutAsync_ShouldSaveOrder_WhenCheckoutIsSuccessful()
    {
        var (customer, product) = CreateCustomerWithProduct();
        SetupCustomerRepositoryMock(customer);
        SetupProductRepositoryMock(product);
        SetupOrderRepositoryMock();
        SetupPaymentRepositoryMock();

        await _checkoutService.CheckoutAsync(customer.Id);

        _orderRepositoryMock.Verify(repo => repo.AddOrderAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task CheckoutAsync_PaymentSaved_AfterSuccessfulCheckout()
    {
        var (customer, product) = CreateCustomerWithProduct();
        SetupCustomerRepositoryMock(customer);
        SetupProductRepositoryMock(product);
        SetupOrderRepositoryMock();
        SetupPaymentRepositoryMock();

            await _checkoutService.CheckoutAsync(customer.Id);

        _paymentRepositoryMock.Verify(repo => repo.AddPaymentAsync(It.IsAny<Payment>()), Times.Once);
    }
}

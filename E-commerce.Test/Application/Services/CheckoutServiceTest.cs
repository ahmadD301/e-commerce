using System;
using System.Threading.Tasks;
using Moq;
using Xunit;


public class CheckoutServiceTest
{   
    [Fact]
    public async Task CheckoutAsync_ShouldPass_WhenAllInputsAreValid()
    {
        // Arrange
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();

        var checkoutService = new CheckoutService(
            customerRepositoryMock.Object, 
            productRepositoryMock.Object, 
            orderRepositoryMock.Object, 
            paymentRepositoryMock.Object
        );

        var customerId = new CustomerId(Guid.NewGuid());
        var customer = new Customer(customerId, "John Doe", "email@domain.com");

        var product = Product.Create("Test Product", new Money(10, "USD"), 100);
        var productId = product.Id;
        customer.Cart.AddItem(product, new Quantity(1));

        customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(customerId))
            .ReturnsAsync(customer);
        productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(productId))
            .ReturnsAsync(product); 
        orderRepositoryMock
            .Setup(repo => repo.AddOrderAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        paymentRepositoryMock
            .Setup(repo => repo.AddPaymentAsync(It.IsAny<Payment>()))
            .Returns(Task.CompletedTask);        
        // Act
        var orderId = await checkoutService.CheckoutAsync(customerId);
        // Assert
        Assert.NotNull(orderId);
    }
    [Fact]
    public async Task CheckoutAsync_ShouldFail_WhenCustomerIsInvalid()
    {
        // Arrange
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();

        var checkoutService = new CheckoutService(
            customerRepositoryMock.Object, 
            productRepositoryMock.Object, 
            orderRepositoryMock.Object, 
            paymentRepositoryMock.Object
        );

        var invalidCustomerId = new CustomerId(Guid.NewGuid());
        customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(invalidCustomerId))
            .ReturnsAsync((Customer)null);

        // Act & Assert
        var message = await Assert.ThrowsAsync<InvalidOperationException>(() => checkoutService.CheckoutAsync(invalidCustomerId));
        Assert.Equal("Customer not found.", message.Message);
    }

    [Fact]
    public async Task CheckoutAsync_ShouldFail_WhenCustomerAccountIsInactive()
    {
        // Arrange 
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();

        var checkoutService = new CheckoutService(
            customerRepositoryMock.Object, 
            productRepositoryMock.Object, 
            orderRepositoryMock.Object, 
            paymentRepositoryMock.Object
        );

        var inactiveCustomerId = new CustomerId(Guid.NewGuid());
        var inactiveCustomer = new Customer(inactiveCustomerId, "John Doe", "Email@domain.com");
        inactiveCustomer.Deactivate();

        customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(inactiveCustomerId))
            .ReturnsAsync(inactiveCustomer);

        // Act & Assert
        var message = await Assert.ThrowsAsync<InvalidOperationException>(() => checkoutService.CheckoutAsync(inactiveCustomerId));
        Assert.Equal("Customer account is inactive.", message.Message);
    }
    [Fact]
    public async Task CheckoutAsync_ShouldFail_WhenShippingCartIsEmpty()
    {
        // Arrange 
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();

        var checkoutService = new CheckoutService(
            customerRepositoryMock.Object, 
            productRepositoryMock.Object, 
            orderRepositoryMock.Object, 
            paymentRepositoryMock.Object
        );
        var customerId = new CustomerId(Guid.NewGuid());
        var customer = new Customer(customerId, "John Doe", "email@domain.com");
        customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(customerId))
            .ReturnsAsync(customer);
        // Act & Assert
        var message = await Assert.ThrowsAsync<InvalidOperationException>(() => checkoutService.CheckoutAsync(customerId));
        Assert.Equal("Shopping cart is empty.", message.Message);
    }
    [Fact]
    public async Task CheckoutAsync_ShouldFail_WhenProductIsInvalid()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();

        var checkoutService = new CheckoutService(
            customerRepositoryMock.Object, 
            productRepositoryMock.Object, 
            orderRepositoryMock.Object, 
            paymentRepositoryMock.Object
        );

        var customerId = new CustomerId(Guid.NewGuid());
        var customer = new Customer(customerId, "John Doe", "email@domain.com");

        var product = Product.Create("Test Product", new Money(10, "USD"), 100);
        var productId = product.Id;

        customer.Cart.AddItem(product, new Quantity(1));

        customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Mock product repository to return null (product not found)
        productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(productId))
            .ReturnsAsync((Product)null);
        
        var message = await Assert.ThrowsAsync<InvalidOperationException>(() => checkoutService.CheckoutAsync(customerId));
        Assert.Equal($"Product with ID {productId} not found.", message.Message);
    }
    [Fact]
    public async Task CheckoutAsync_ShouldFail_WhenProductIsOutOfStock()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();

        var checkoutService = new CheckoutService(
            customerRepositoryMock.Object, 
            productRepositoryMock.Object, 
            orderRepositoryMock.Object, 
            paymentRepositoryMock.Object
        );

        var customerId = new CustomerId(Guid.NewGuid());
        var customer = new Customer(customerId, "John Doe", "email@domain.com");

        var product = Product.Create("Test Product", new Money(10, "USD"), 0); // Out of stock
        var productId = product.Id;

        customer.Cart.AddItem(product, new Quantity(1));
        customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(customerId))
            .ReturnsAsync(customer);
        productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(productId))
            .ReturnsAsync(product);
        
        var message = await Assert.ThrowsAsync<InvalidOperationException>(() => checkoutService.CheckoutAsync(customerId));
        Assert.Equal($"Insufficient stock for product {product.Name}.", message.Message);
    }
    [Fact]
    public async Task CheckoutAsync_ShouldFail_WhenPaymentFails()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();

        var checkoutService = new CheckoutService(
            customerRepositoryMock.Object, 
            productRepositoryMock.Object, 
            orderRepositoryMock.Object, 
            paymentRepositoryMock.Object
        );
        var customerId = new CustomerId(Guid.NewGuid());
        var customer = new Customer(customerId, "John Doe", "email@domain.com");

        var product = Product.Create("Test Product", new Money(10, "USD"), 100);
        var productId = product.Id;
        customer.Cart.AddItem(product, new Quantity(1));
        customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(customerId))
            .ReturnsAsync(customer);
        productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(productId))
            .ReturnsAsync(product);
        orderRepositoryMock
            .Setup(repo => repo.AddOrderAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        // Simulate payment failure by throwing an exception in the payment repository
        paymentRepositoryMock
            .Setup(repo => repo.AddPaymentAsync(It.IsAny<Payment>()))
            .ThrowsAsync(new InvalidOperationException("Payment failed."));
        
        // Act & Assert
        var message = await Assert.ThrowsAsync<InvalidOperationException>(() => checkoutService.CheckoutAsync(customerId));
        Assert.Equal("Payment failed.", message.Message);
    }
    [Fact]
    public async Task CheckoutAsync_ShouldReduceProductStock_WhenCheckoutIsSuccessful()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();

        var checkoutService = new CheckoutService(
            customerRepositoryMock.Object, 
            productRepositoryMock.Object, 
            orderRepositoryMock.Object, 
            paymentRepositoryMock.Object
        );

        var customerId = new CustomerId(Guid.NewGuid());
        var customer = new Customer(customerId, "John Doe", "email@domain.com");

        var productStock = 100;
        var productOrderedQuantity = 1;
        var productStockAfterOrder = productStock - productOrderedQuantity;

        var product = Product.Create("Test Product", new Money(10, "USD"), productStock);
        var productId = product.Id;
        customer.Cart.AddItem(product, new Quantity(1));

        customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(customerId))
            .ReturnsAsync(customer);
        productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(productId))
            .ReturnsAsync(product);
        orderRepositoryMock
            .Setup(repo => repo.AddOrderAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        paymentRepositoryMock
            .Setup(repo => repo.AddPaymentAsync(It.IsAny<Payment>()))
            .Returns(Task.CompletedTask);

        // Act
        await checkoutService.CheckoutAsync(customerId);
        
        // Assert
        Assert.Equal(productStockAfterOrder, product.Stock);
    }
    [Fact]
    public async Task CheckoutAsync_ShouldClearCart_AfterSuccessfulCheckout()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();

        var checkoutService = new CheckoutService(
            customerRepositoryMock.Object, 
            productRepositoryMock.Object, 
            orderRepositoryMock.Object, 
            paymentRepositoryMock.Object
        );
        
        var customerId = new CustomerId(Guid.NewGuid());
        var customer = new Customer(customerId, "John Doe", "email@domain.com");

        var product = Product.Create("Test Product", new Money(10, "USD"), 100);
        var productId = product.Id;
        customer.Cart.AddItem(product, new Quantity(1));

        customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(customerId))
            .ReturnsAsync(customer);
        productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(productId))
            .ReturnsAsync(product); 
        orderRepositoryMock
            .Setup(repo => repo.AddOrderAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        paymentRepositoryMock
            .Setup(repo => repo.AddPaymentAsync(It.IsAny<Payment>()))
            .Returns(Task.CompletedTask);        

        // Act
        await checkoutService.CheckoutAsync(customerId);
        // Assert
        Assert.Empty(customer.Cart.Items);
    }
    [Fact]
    public async Task CheckoutAsync_ShouldSaveOrder_WhenCheckoutIsSuccessful()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();

        var checkoutService = new CheckoutService(
            customerRepositoryMock.Object, 
            productRepositoryMock.Object, 
            orderRepositoryMock.Object, 
            paymentRepositoryMock.Object
        );
        
        var customerId = new CustomerId(Guid.NewGuid());
        var customer = new Customer(customerId, "John Doe", "email@domain.com");

        var product = Product.Create("Test Product", new Money(10, "USD"), 100);
        var productId = product.Id;
        customer.Cart.AddItem(product, new Quantity(1));

        customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(customerId))
            .ReturnsAsync(customer);
        productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(productId))
            .ReturnsAsync(product); 
        orderRepositoryMock
            .Setup(repo => repo.AddOrderAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        paymentRepositoryMock
            .Setup(repo => repo.AddPaymentAsync(It.IsAny<Payment>()))
            .Returns(Task.CompletedTask);   
        
        // Act
        await checkoutService.CheckoutAsync(customerId);
        // Assert
        orderRepositoryMock.Verify(repo => repo.AddOrderAsync(It.IsAny<Order>()), Times.Once);
    }
    [Fact]
    public async Task CheckoutAsync_PaymentSaved_AfterSuccessfulCheckout()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();

        var checkoutService = new CheckoutService(
            customerRepositoryMock.Object, 
            productRepositoryMock.Object, 
            orderRepositoryMock.Object, 
            paymentRepositoryMock.Object
        );
        
        var customerId = new CustomerId(Guid.NewGuid());
        var customer = new Customer(customerId, "John Doe", "email@domain.com");

        var product = Product.Create("Test Product", new Money(10, "USD"), 100);
        var productId = product.Id;
        customer.Cart.AddItem(product, new Quantity(1));
        customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(customerId))
            .ReturnsAsync(customer);
        productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(productId))
            .ReturnsAsync(product);
        orderRepositoryMock
            .Setup(repo => repo.AddOrderAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        paymentRepositoryMock  
            .Setup(repo => repo.AddPaymentAsync(It.IsAny<Payment>()))
            .Returns(Task.CompletedTask);
        
        // Act
        await checkoutService.CheckoutAsync(customerId); 

        // Assert
        paymentRepositoryMock.Verify(repo => repo.AddPaymentAsync(It.IsAny<Payment>()), Times.Once);
    }
}
public class CustomerTest
{
    [Fact]
    public void CreateCustomer_ShouldInitializeProperties()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        
        // Act
        var customer = Customer.Create(name, email);
        // Assert
        Assert.NotNull(customer);
        Assert.Equal(name, customer.Name);
        Assert.Equal(email, customer.Email);
        Assert.NotNull(customer.Cart);
        Assert.True(customer.IsActive);
    }
    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var customer = Customer.Create(name, email);

        // Act
        customer.Deactivate();

        // Assert
        Assert.False(customer.IsActive);
    }
        [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var customer = Customer.Create(name, email);
        customer.Deactivate();

        // Act
        customer.Activate();

        // Assert
        Assert.True(customer.IsActive);
    }       
    [Fact]
    public void Checkout_ShouldFail_WhenCartIsEmpty()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var customer = Customer.Create(name, email);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => customer.checkout());
        Assert.Equal("Cannot checkout an empty cart.", exception.Message);
    }
    [Fact]
    public void Checkout_ShouldFail_WhenCustomerIsInactive()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var customer = Customer.Create(name, email);
        customer.Deactivate();

        var product = Product.Create("Test Product", new Money(10.0m , "USD"), 100);
        customer.Cart.AddItem(product, new Quantity(1));
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => customer.checkout());
        Assert.Equal("Cannot checkout with an inactive customer.", exception.Message);
    }
    [Fact]
    public void Checkout_ShouldSucceed_WhenCustomerIsActiveAndCartIsNotEmpty()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var customer = Customer.Create(name, email);

        var product = Product.Create("Test Product", new Money(10.0m , "USD"), 100);
        customer.Cart.AddItem(product, new Quantity(1));
        
        // Act
        customer.checkout(); 
        // Assert
        // Application Service will handle the actual checkout process, 
        // we just want to ensure it doesn't throw exceptions here

        Assert.True(true); // If we reach this point, the test is successful
    }
    
}
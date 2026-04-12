public class ProductTest
{
    private Product CreateValidProduct()
    {
        return Product.Create("Test Product", new Money(10.00m, "USD"), 100);
    }
    [Fact]
    public void Create_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var name = "Test Product";
        var price = new Money(10.00m , "USD");
        var initialStock = 100;

        // Act
        var product = Product.Create(name, price, initialStock);

        // Assert
        Assert.NotNull(product);
        Assert.Equal(name, product.Name);
        Assert.Equal(price, product.Price);
        Assert.Equal(initialStock, product.Stock);
        Assert.True(product.IsActive);
    }
    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameIsNull()
    {
        // Arrange
        string name = null;
        var price = new Money(10.00m , "USD");
        var initialStock = 100;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Product.Create(name, price, initialStock));
        Assert.Equal("Product name cannot be empty. (Parameter 'name')", ex.Message);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenStockIsNegative()
    {
        // Arrange
        var name = "Test Product";
        var price = new Money(10.00m , "USD");
        var initialStock = -1;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Product.Create(name, price, initialStock));
        Assert.Equal("Stock cannot be negative. (Parameter 'stock')", ex.Message);
    }

    [Fact]
    public void ChangePrice_ShouldUpdatePrice_WhenCalled()
    {
        // Arrange
        var product = CreateValidProduct();
        var newPrice = new Money(15.00m, "USD");

        // Act
        product.ChangePrice(newPrice);

        // Assert
        Assert.Equal(newPrice, product.Price);
    }
    [Fact]
    public void IncreaseStock_ShouldUpdateStock_WhenCalled()
    {
        // Arrange
        var product = CreateValidProduct();
        var amountToIncrease = 50;

        // Act
        product.IncreaseStock(amountToIncrease);

        // Assert
        Assert.Equal(150, product.Stock);
    }
    [Fact]
    public void IncreaseStock_ShouldInvalidOperationException_ProductIsInactive()
    {
        // Arrange
        var product = CreateValidProduct();
        product.Deactivate();
        var amountToIncrease = new Quantity(10);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => product.IncreaseStock(amountToIncrease));
        Assert.Equal("Cannot increase stock of inactive product.", ex.Message);
    }
    [Fact]
    public void IncreaseStock_ShouldInvalidOperationException_WhenQuantityIsNotPositive()
    {
        // Given
        var product = CreateValidProduct();
        var amountToIncrease = new Quantity(0);

        // When & Then
        var ex = Assert.Throws<ArgumentException>(() => product.IncreaseStock(amountToIncrease));
        Assert.Equal("Quantity must be greater than zero. (Parameter 'quantity')", ex.Message);
    }
    [Fact]
    public void DecreaseStock_ShouldUpdateStock_WhenCalled()
    {
        // Arrange
        var product = CreateValidProduct();
        var amountToDecrease = new Quantity(50);

        // Act
        product.DecreaseStock(amountToDecrease);
        // Assert
        Assert.Equal(50, product.Stock);
    }
    [Fact]
    public void DecreaseStock_ShouldInvalidOperationException_WhenProductIsInactive()
    {
        // Arrange
        var product = CreateValidProduct();
        product.Deactivate();
        var amountToDecrease = new Quantity(50);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => product.DecreaseStock(amountToDecrease));
        Assert.Equal("Cannot decrease stock of inactive product.", ex.Message);
    }
    [Fact]
    public void DecreaseStock_ShouldInvalidOperationException_WhenInsufficientStock()
    {
        // Arrange
        var product = CreateValidProduct();
        var amountToDecrease = new Quantity(150);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => product.DecreaseStock(amountToDecrease));
        Assert.Equal("Insufficient stock.", ex.Message);
    }
    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        product.Deactivate();

        // Assert
        Assert.False(product.IsActive);
    }
    [Fact]

    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var product = CreateValidProduct();
        product.Deactivate();

        // Act
        product.Activate();

        // Assert
        Assert.True(product.IsActive);
    }
    
}

public class CartTest
{
    [Fact]
    public void AddItem_ShouldSucceed_WhenItemIsAdded()
    {
        // Arrange
        var product = Product.Create("Test Product", new Money(10.00m, "USD"), 10);
        var quantity = new Quantity(2);
        // Act
        var cart = new Cart();
        cart.AddItem(product, quantity);
        // Assert
        Assert.Single(cart.Items);
        var item = cart.Items.First();
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(product.Name, item.ProductName);
        Assert.Equal(product.Price, item.UnitPrice);
        Assert.Equal(quantity, item.Quantity);
        
    }
    [Fact]
    public void AddItem_ShouldFail_WhenItemInActive()
    {
        // Arrange
        var product = Product.Create("Test Product", new Money(10.00m, "USD"), 10);
        var quantity = new Quantity(2);
        product.Deactivate();
        // Act
        var cart = new Cart();
        // Assert
        var exception = Assert.Throws<ArgumentNullException>(() => cart.AddItem(product, quantity));
        Assert.Equal("Cannot add inactive product to cart. (Parameter 'product')", exception.Message);
    }
    [Fact]
    public void RemoveItem_ShouldSucceed_WhenItemIsRemoved()
    {
        // Arrange
        var product = Product.Create("Test Product", new Money(10.00m, "USD"), 10);
        var quantity = new Quantity(2);
        var cart = new Cart();
        cart.AddItem(product, quantity);

        // Act
        cart.RemoveItem(product.Id);
        // Assert
        Assert.Empty(cart.Items);
    }
    [Fact]
    public void RemoveItem_ShouldFail_WhenItemIsNotInCart()
    {
        // Arrange
        var product = Product.Create("Test Product", new Money(10.00m, "USD"), 10);
        var cart = new Cart();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => cart.RemoveItem(product.Id));
        Assert.Equal("Product not found in cart. (Parameter 'productId')", exception.Message);
    }
    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        // Arrange
        var product = Product.Create("Test Product", new Money(10.00m, "USD"), 10);
        var quantity = new Quantity(2);
        var cart = new Cart();
        cart.AddItem(product, quantity);

        // Act
        cart.Clear();
        // Assert
        Assert.Empty(cart.Items);
    }
    
    [Fact]
    public void TotalAmount_ShouldCalculateCorrectly()
    {
        // Arrange
        var product1 = Product.Create("Product 1", new Money(10.00m, "USD"), 10);
        var product2 = Product.Create("Product 2", new Money(20.00m, "USD"), 10);
        var cart = new Cart();
        cart.AddItem(product1, new Quantity(2)); // $20
        cart.AddItem(product2, new Quantity(1)); // $20

        // Act
        var totalAmount = cart.TotalAmount;

        // Assert
        Assert.Equal(new Money(40.00m, "USD"), totalAmount);
    }
}
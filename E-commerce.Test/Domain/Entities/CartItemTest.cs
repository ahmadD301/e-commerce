public class CartItemTest
{
    [Fact]
    public void CreateCartItem_ShouldSucceed()
    {
        // Arrange
            var productId = new ProductId(Guid.NewGuid());
            var productName = "Test Product";
            var price = new Money(10.0m, "USD");
            var quantity = new Quantity(2);
        // Act
            var cartItem = new CartItem(productId, productName, price, quantity);
        // Assert
            Assert.Equal(productId, cartItem.ProductId);
            Assert.Equal(productName, cartItem.ProductName);
            Assert.Equal(price, cartItem.UnitPrice);
            Assert.Equal(quantity, cartItem.Quantity);
            Assert.Equal(new Money(20.0m, "USD"), cartItem.TotalPrice);
    }
    [Fact]
    public void IncreaseQuantity_ShouldSucceed()
    {
        // Arrange
            var productId = new ProductId(Guid.NewGuid());
            var productName = "Test Product";
            var price = new Money(10.0m, "USD");
            var quantity = new Quantity(2);
        // Act
            var cartItem = new CartItem(productId, productName, price, quantity);
            cartItem.IncreaseQuantity(new Quantity(3));
        // Assert
            Assert.Equal(new Quantity(5), cartItem.Quantity);
            Assert.Equal(new Money(50.0m, "USD"), cartItem.TotalPrice);
    }
    [Fact]
    public void DecreaseQuantity_ShouldSucceed()
    {
        // Arrange
            var productId = new ProductId(Guid.NewGuid());
            var productName = "Test Product";
            var price = new Money(10.0m, "USD");
            var quantity = new Quantity(5);
        // Act
            var cartItem = new CartItem(productId, productName, price, quantity);
            cartItem.DecreaseQuantity(new Quantity(2));
        // Assert
            Assert.Equal(new Quantity(3), cartItem.Quantity);
            Assert.Equal(new Money(30.0m, "USD"), cartItem.TotalPrice);
    }
}
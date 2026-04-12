public class OrderItemTest
{
    [Fact]
    public void CreateOrderItem_ShouldSucceed()
    {
        // Arrange
            var productId = new ProductId(Guid.NewGuid());
            var productName = "Test Product";
            var price = new Money(10.0m, "USD");
            var quantity = new Quantity(2);
        // Act
            var orderItem = new OrderItem(productId, productName, price, quantity);
        // Assert
            Assert.Equal(productId, orderItem.ProductId);
            Assert.Equal(productName, orderItem.ProductName);
            Assert.Equal(price, orderItem.Price);
            Assert.Equal(quantity, orderItem.Quantity);
            Assert.Equal(new Money(20.0m, "USD"), orderItem.TotalPrice);
    }
}
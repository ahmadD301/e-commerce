public class OrderTest
    {
    [Fact]
    public void Create_ShouldInitializeOrderWithGivenDetails()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 1", new Money(10 , "USD") , new Quantity(2)),
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 2", new Money(20 , "USD") , new Quantity(1))
        };

        // Act
        var order = Order.Create(customerId, items);
        // Assert
        Assert.NotNull(order);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(2, order.Items.Count);
        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void Create_ShouldFail_WhenOrderItemsAreEmpty()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Order.Create(customerId, items));
        Assert.Equal("Order must contain at least one item.", exception.Message);
    }

    [Fact]
    public void CalculateTotalAmount_ShouldReturnCorrectTotal()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 1", new Money(10 , "USD") , new Quantity(2)),
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 2", new Money(20 , "USD") , new Quantity(1))
        };
        
        // Act
        var totalAmount = Order.Create(customerId, items).CalculateTotalAmount();
        
        // Assert
        Assert.Equal(new Money(40, "USD"), totalAmount);

    }
    [Fact]
    public void MarkAsPaid_ShouldUpdateOrderStatus()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 1", new Money(10 , "USD") , new Quantity(2)),
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 2", new Money(20 , "USD") , new Quantity(1))
        };
        var order = Order.Create(customerId, items);

        // Act
        order.MarkAsPaid();
        // Assert
        Assert.Equal(OrderStatus.Paid, order.Status);

    }
    [Fact]
    public void MarkAsPaid_ShouldThrowException_WhenOrderIsPending()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 1", new Money(10 , "USD") , new Quantity(2)),
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 2", new Money(20 , "USD") , new Quantity(1))
        };
        var order = Order.Create(customerId, items);
        order.MarkAsPaid();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => order.MarkAsPaid());
        Assert.Equal("Only pending orders can be marked as paid.", exception.Message);
    }
    [Fact]
    public void CancelOrder_ShouldUpdateOrderStatus()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 1", new Money(10 , "USD") , new Quantity(2)),
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 2", new Money(20 , "USD") , new Quantity(1))
        };
        var order = Order.Create(customerId, items);

        // Act
        order.Cancel();
        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);

    }
        [Fact]
    public void Cancel_ShouldThrowException_WhenOrderIsAlreadyPaid()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 1", new Money(10 , "USD") , new Quantity(2)),
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 2", new Money(20 , "USD") , new Quantity(1))
        };
        var order = Order.Create(customerId, items);
        order.MarkAsPaid();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => order.Cancel());
        Assert.Equal("Cannot cancel a paid order.", exception.Message);
    }
        [Fact]
    public void AddItem_ShouldAddItemToOrder()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 1", new Money(10 , "USD") , new Quantity(2)),
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 2", new Money(20 , "USD") , new Quantity(1))
        };
        var order = Order.Create(customerId, items);
        var newItem = new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 3", new Money(15 , "USD") , new Quantity(1))
        }.First();
        // Act
        order.AddItem(newItem);
        // Assert
        Assert.Equal(3, order.Items.Count);
        Assert.Contains(newItem, order.Items);
        
    }
    [Fact]
    public void RemoveItem_ShouldRemoveItemFromOrder()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 1", new Money(10 , "USD") , new Quantity(2)),
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 2", new Money(20 , "USD") , new Quantity(1))
        };
        var order = Order.Create(customerId, items);
        var itemToRemove = order.Items.First();

        // Act
        order.RemoveItem(itemToRemove.ProductId);
        // Assert
        Assert.Single(order.Items);
        Assert.DoesNotContain(itemToRemove, order.Items);
    }
    [Fact]
    public void RemoveItem_ShouldThrowException_WhenItemIsNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 1", new Money(10 , "USD") , new Quantity(2)),
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 2", new Money(20 , "USD") , new Quantity(1))
        };
        var order = Order.Create(customerId, items);
        var nonExistentProductId = new ProductId(Guid.NewGuid());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => order.RemoveItem(nonExistentProductId));
        Assert.Equal("Item not found in order. (Parameter 'productId')", exception.Message);
    }

    [Fact]
    public void RemoveItem_ShouldThrowException_WhenOrderStatusIsNotPending()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 1", new Money(10 , "USD") , new Quantity(2)),
            new OrderItem(new ProductId(Guid.NewGuid()), "Product 2", new Money(20 , "USD") , new Quantity(1))
        };
        var order = Order.Create(customerId, items);
        order.MarkAsPaid();
        var itemToRemove = order.Items.First();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => order.RemoveItem(itemToRemove.ProductId));
        Assert.Equal("Can only remove items from pending orders.", exception.Message);
    }
}
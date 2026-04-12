public class PaymentTest
{
    [Fact]
    public void Create_WithValidPendingOrder_ShouldCreatePaymentWithPendingStatus()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
        });
        // Act
        var payment = Payment.Create(order);
        // Assert
        Assert.NotNull(payment);
        Assert.Equal(order.TotalAmount, payment.Amount);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
    }
    [Fact]
    public void Create_WithNonPendingOrder_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
        });
        order.MarkAsPaid();
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => Payment.Create(order));
        Assert.Equal("Payment can only be created for pending orders.", ex.Message);
    }
    [Fact]
    public void MarkAsCompleted_WhenPaymentIsAlreadyCompleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
        });
        var payment = Payment.Create(order);
        payment.MarkAsCompleted();
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => payment.MarkAsCompleted());
        Assert.Equal("Only pending payments can be marked as completed.", ex.Message);
    }
    [Fact]
    public void MarkAsCompleted_WhenPaymentIsFailed_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
        });
        var payment = Payment.Create(order);
        payment.MarkAsFailed();
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => payment.MarkAsCompleted());
        Assert.Equal("Only pending payments can be marked as completed.", ex.Message);
    }
    [Fact]
    public void MarkAsFailed_WhenPaymentIsPending_ShouldChangeStatusToFailed()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
        });
        var payment = Payment.Create(order);

        // Act
        payment.MarkAsFailed();

        // Assert
        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.False(payment.IsCompleted);
    }
    [Fact]
    public void MarkAsFailed_WhenPaymentIsAlreadyCompleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
        });
        var payment = Payment.Create(order);
        payment.MarkAsCompleted();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => payment.MarkAsFailed());
        Assert.Equal("Only pending payments can be marked as failed.", ex.Message);
    }
    [Fact]
    public void Cancel_WhenPaymentIsPending_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
        });
        var payment = Payment.Create(order);

        // Act
        payment.Cancel();

        // Assert
        Assert.Equal(PaymentStatus.Cancelled, payment.Status);
    }

    [Fact]
    public void Cancel_WhenPaymentIsFailed_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
        });
        var payment = Payment.Create(order);
        payment.MarkAsFailed();

        // Act
        payment.Cancel();

        // Assert
        Assert.Equal(PaymentStatus.Cancelled, payment.Status);
    }

    [Fact]
    public void Cancel_WhenPaymentIsCompleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
        });
        var payment = Payment.Create(order);
        payment.MarkAsCompleted();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => payment.Cancel());
        Assert.Equal("Cannot cancel a completed payment.", ex.Message);
    }

    [Fact]
    public void IsCompleted_ShouldReturnTrueOnlyForCompletedStatus()
    {
        // Test Pending status
        var order1 = Order.Create(Guid.NewGuid(), new List<OrderItem>
    {
        new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
    });
        var payment1 = Payment.Create(order1);
        Assert.False(payment1.IsCompleted);

        // Test Completed status
        var order2 = Order.Create(Guid.NewGuid(), new List<OrderItem>
    {
        new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
    });
        var payment2 = Payment.Create(order2);
        payment2.MarkAsCompleted();
        Assert.True(payment2.IsCompleted);

        // Test Failed status
        var order3 = Order.Create(Guid.NewGuid(), new List<OrderItem>
    {
        new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
    });
        var payment3 = Payment.Create(order3);
        payment3.MarkAsFailed();
        Assert.False(payment3.IsCompleted);

        // Test Cancelled status
        var order4 = Order.Create(Guid.NewGuid(), new List<OrderItem>
    {
        new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
    });
        var payment4 = Payment.Create(order4);
        payment4.Cancel();
        Assert.False(payment4.IsCompleted);
    }

    // Helper methods
    private Order CreatePendingOrder()
    {
        return Order.Create(Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
        });
    }

    private Order CreateCompletedOrder()
    {
        var order = Order.Create(Guid.NewGuid(), new List<OrderItem>
        {
            new OrderItem(new ProductId(Guid.NewGuid()), "Test Product", new Money(10 , "USD"), new Quantity(2))
        });
        order.MarkAsPaid();
        return order;
    }
}
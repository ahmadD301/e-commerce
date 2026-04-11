

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly Dictionary<CustomerId, Customer> _customers = new();

    public Task<Customer?> GetCustomerByIdAsync(CustomerId id)
    {
        _customers.TryGetValue(id, out var customer);
        return Task.FromResult(customer);
    }

    public Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
        return Task.FromResult<IEnumerable<Customer>>(_customers.Values.ToList());
    }

    public Task AddCustomerAsync(Customer customer)
    {
        _customers[customer.Id] = customer;
        return Task.CompletedTask;
    }

    public Task UpdateCustomerAsync(Customer customer)
    {
        _customers[customer.Id] = customer;
        return Task.CompletedTask;
    }
}

public class InMemoryProductRepository : IProductRepository
{
    private readonly Dictionary<ProductId, Product> _products = new();

    public Task<Product?> GetProductByIdAsync(ProductId id)
    {
        _products.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return Task.FromResult<IEnumerable<Product>>(_products.Values.ToList());
    }

    public Task AddProductAsync(Product product)
    {
        _products[product.Id] = product;
        return Task.CompletedTask;
    }

    public Task UpdateProductAsync(Product product)
    {
        _products[product.Id] = product;
        return Task.CompletedTask;
    }
}

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<OrderId, Order> _orders = new();

    public Task<Order?> GetOrderByIdAsync(OrderId id)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return Task.FromResult<IEnumerable<Order>>(_orders.Values.ToList());
    }

    public Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId)
    {
        var orders = _orders.Values.Where(o => o.CustomerId == customerId).ToList();
        return Task.FromResult<IEnumerable<Order>>(orders);
    }

    public Task AddOrderAsync(Order order)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task UpdateOrderAsync(Order order)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }
}

public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly Dictionary<PaymentId, Payment> _payments = new();

    public Task<Payment?> GetPaymentByIdAsync(PaymentId id)
    {
        _payments.TryGetValue(id, out var payment);
        return Task.FromResult(payment);
    }

    public Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        return Task.FromResult<IEnumerable<Payment>>(_payments.Values.ToList());
    }

    public Task AddPaymentAsync(Payment payment)
    {
        _payments[payment.Id] = payment;
        return Task.CompletedTask;
    }

    public Task UpdatePaymentAsync(Payment payment)
    {
        _payments[payment.Id] = payment;
        return Task.CompletedTask;
    }
}


public interface ICustomerRepository
{
    Task<Customer?> GetCustomerByIdAsync(CustomerId id);
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task AddCustomerAsync(Customer customer);
    Task UpdateCustomerAsync(Customer customer);
}

public interface IProductRepository
{
    Task<Product?> GetProductByIdAsync(ProductId id);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
}

public interface IOrderRepository
{
    Task<Order?> GetOrderByIdAsync(OrderId id);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId);
    Task AddOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
}

public interface IPaymentRepository
{
    Task<Payment?> GetPaymentByIdAsync(PaymentId id);
    Task<IEnumerable<Payment>> GetAllPaymentsAsync();
    Task AddPaymentAsync(Payment payment);
    Task UpdatePaymentAsync(Payment payment);
}
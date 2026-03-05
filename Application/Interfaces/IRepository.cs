

public interface ICustomerRepository
{
    Task<Customer?> GetCustomerByIdAsync(CustomerId id);
    Task AddCustomerAsync(Customer customer);
    Task UpdateCustomerAsync(Customer customer);
}

public interface IProductRepository
{
    Task<Product?> GetProductByIdAsync(ProductId id);
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
}

public interface IOrderRepository
{
    Task<Order?> GetOrderByIdAsync(OrderId id);
    Task AddOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
}

public interface IPaymentRepository
{
    Task<Payment?> GetPaymentByIdAsync(PaymentId id);
    Task AddPaymentAsync(Payment payment);
    Task UpdatePaymentAsync(Payment payment);
}

using System;
using System.Collections.Generic;
public class CheckoutService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;

    public CheckoutService(
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<OrderId> CheckoutAsync(CustomerId customerId)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
        if (!customer.IsActive)
        {
            throw new InvalidOperationException("Customer account is inactive.");
        }
        if (customer.Cart.IsEmpty)
        {
            throw new InvalidOperationException("Shopping cart is empty.");
        }
        var orderItem = new List<OrderItem>();
        foreach (var cartItem in customer.Cart.Items)
        {
            var product = await _productRepository.GetProductByIdAsync(cartItem.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {cartItem.ProductId} not found.");
            }
            if (product.Stock < cartItem.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}.");
            }
            orderItem.Add(new OrderItem(product.Id, product.Name, product.Price, cartItem.Quantity));
        }

        var order = Order.Create(customerId.Value , orderItem);
        await _orderRepository.AddOrderAsync(order);

        var payment = Payment.Create(order);
        await _paymentRepository.AddPaymentAsync(payment);

        var paymentResult = SimulatePaymentGateway();
    
        if (!paymentResult)
        {
            payment.MarkAsFailed();
            await _paymentRepository.UpdatePaymentAsync(payment);
            throw new InvalidOperationException("Payment failed. Please try again.");
        }
        payment.MarkAsCompleted();
        order.MarkAsPaid();
    
        foreach (var item in order.Items)
        {
            var product = await _productRepository.GetProductByIdAsync(item.ProductId)
            ?? throw new InvalidOperationException($"Product with ID {item.ProductId} not found.");
            
            product.DecreaseStock(item.Quantity);
            await _productRepository.UpdateProductAsync(product);
        }
    

        customer.Cart.Clear();

        await _customerRepository.UpdateCustomerAsync(customer);
        await _orderRepository.UpdateOrderAsync(order);
        await _paymentRepository.UpdatePaymentAsync(payment);

        return order.Id;
    
    }

    private bool SimulatePaymentGateway()
    {
        // 90% success simulation
        return Random.Shared.Next(1, 11) <= 9;
    }
}
using System;

// Initialize repositories
var customerRepository = new InMemoryCustomerRepository();
var productRepository = new InMemoryProductRepository();
var orderRepository = new InMemoryOrderRepository();
var paymentRepository = new InMemoryPaymentRepository();

// Initialize services
var checkoutService = new CheckoutService(
    customerRepository,
    productRepository,
    orderRepository,
    paymentRepository);

// Seed initial data
await DataSeeder.SeedDataAsync(customerRepository, productRepository);

// Start CLI
var cli = new ECommerceCLI(
    customerRepository,
    productRepository,
    orderRepository,
    paymentRepository,
    checkoutService);

await cli.RunAsync();

// ============================================
// CLI Application
// ============================================
public class ECommerceCLI
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly CheckoutService _checkoutService;
    private Customer? _currentCustomer;

    public ECommerceCLI(
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        CheckoutService checkoutService)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _checkoutService = checkoutService;
    }

    public async Task RunAsync()
    {
        Console.Clear();
        PrintHeader();

        while (true)
        {
            try
            {
                PrintMainMenu();
                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        await ProductManagementMenuAsync();
                        break;
                    case "2":
                        await CustomerManagementMenuAsync();
                        break;
                    case "3":
                        await ShoppingMenuAsync();
                        break;
                    case "4":
                        await OrderManagementMenuAsync();
                        break;
                    case "5":
                        Console.WriteLine("\n👋 Thank you for using E-Commerce System!");
                        return;
                    default:
                        Console.WriteLine("\n❌ Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private void PrintHeader()
    {
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║     E-COMMERCE MANAGEMENT SYSTEM       ║");
        Console.WriteLine("║          DDD Architecture              ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();
    }

    private void PrintMainMenu()
    {
        Console.Clear();
        PrintHeader();
        
        if (_currentCustomer != null)
        {
            Console.WriteLine($"🛒 Logged in as: {_currentCustomer.Name} ({_currentCustomer.Email})");
            Console.WriteLine($"   Cart Items: {_currentCustomer.Cart.Items.Count}");
            Console.WriteLine();
        }

        Console.WriteLine("┌─────────────────────────────────────┐");
        Console.WriteLine("│          MAIN MENU                  │");
        Console.WriteLine("├─────────────────────────────────────┤");
        Console.WriteLine("│ 1. 📦 Product Management            │");
        Console.WriteLine("│ 2. 👤 Customer Management           │");
        Console.WriteLine("│ 3. 🛍️  Shopping                      │");
        Console.WriteLine("│ 4. 📋 Order Management              │");
        Console.WriteLine("│ 5. 🚪 Exit                          │");
        Console.WriteLine("└─────────────────────────────────────┘");
        Console.Write("\nEnter your choice: ");
    }

    // ============================================
    // PRODUCT MANAGEMENT
    // ============================================
    private async Task ProductManagementMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║      📦 PRODUCT MANAGEMENT             ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");
            Console.WriteLine("1. List All Products");
            Console.WriteLine("2. View Product Details");
            Console.WriteLine("3. Add New Product");
            Console.WriteLine("4. Update Product Stock");
            Console.WriteLine("5. Activate/Deactivate Product");
            Console.WriteLine("6. Back to Main Menu");
            Console.Write("\nEnter your choice: ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    await ListAllProductsAsync();
                    break;
                case "2":
                    await ViewProductDetailsAsync();
                    break;
                case "3":
                    await AddNewProductAsync();
                    break;
                case "4":
                    await UpdateProductStockAsync();
                    break;
                case "5":
                    await ToggleProductStatusAsync();
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("\n❌ Invalid choice.");
                    break;
            }

            if (choice != "6")
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private async Task ListAllProductsAsync()
    {
        var products = await _productRepository.GetAllProductsAsync();
        Console.WriteLine("\n📦 PRODUCT CATALOG");
        Console.WriteLine("═════════════════════════════════════════════════════════════════");
        Console.WriteLine($"{"ID",-38} {"Name",-30} {"Price",-15} {"Stock",-8} {"Status"}");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");

        foreach (var product in products)
        {
            var status = product.IsActive ? "✓ Active" : "✗ Inactive";
            Console.WriteLine($"{product.Id.ToString(),-38} {TruncateString(product.Name, 28),-30} {product.Price,-15} {product.Stock,-8} {status}");
        }

        Console.WriteLine("═════════════════════════════════════════════════════════════════");
        Console.WriteLine($"Total Products: {products.Count()}");
    }

    private async Task ViewProductDetailsAsync()
    {
        Console.Write("\nEnter Product ID: ");
        var idInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(idInput) || !Guid.TryParse(idInput, out var id))
        {
            Console.WriteLine("❌ Invalid Product ID.");
            return;
        }

        var product = await _productRepository.GetProductByIdAsync(new ProductId(id));
        if (product == null)
        {
            Console.WriteLine("❌ Product not found.");
            return;
        }

        Console.WriteLine("\n📦 PRODUCT DETAILS");
        Console.WriteLine("═════════════════════════════════════");
        Console.WriteLine($"ID:          {product.Id}");
        Console.WriteLine($"Name:        {product.Name}");
        Console.WriteLine($"Price:       {product.Price}");
        Console.WriteLine($"Stock:       {product.Stock} units");
        Console.WriteLine($"Status:      {(product.IsActive ? "✓ Active" : "✗ Inactive")}");
        Console.WriteLine($"Availability: {(product.Stock > 0 ? "In Stock" : "Out of Stock")}");
        Console.WriteLine("═════════════════════════════════════");
    }

    private async Task AddNewProductAsync()
    {
        Console.WriteLine("\n➕ ADD NEW PRODUCT");
        Console.WriteLine("═════════════════════════════════════");

        Console.Write("Product Name: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("❌ Product name cannot be empty.");
            return;
        }

        Console.Write("Price (USD): ");
        if (!decimal.TryParse(Console.ReadLine(), out var price) || price < 0)
        {
            Console.WriteLine("❌ Invalid price.");
            return;
        }

        Console.Write("Initial Stock: ");
        if (!int.TryParse(Console.ReadLine(), out var stock) || stock < 0)
        {
            Console.WriteLine("❌ Invalid stock quantity.");
            return;
        }

        var product = Product.Create(name, new Money(price, "USD"), stock);
        await _productRepository.AddProductAsync(product);

        Console.WriteLine($"\n✓ Product '{name}' added successfully!");
        Console.WriteLine($"  Product ID: {product.Id}");
    }

    private async Task UpdateProductStockAsync()
    {
        Console.Write("\nEnter Product ID: ");
        var idInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(idInput) || !Guid.TryParse(idInput, out var id))
        {
            Console.WriteLine("❌ Invalid Product ID.");
            return;
        }

        var product = await _productRepository.GetProductByIdAsync(new ProductId(id));
        if (product == null)
        {
            Console.WriteLine("❌ Product not found.");
            return;
        }

        Console.WriteLine($"\nCurrent Stock: {product.Stock} units");
        Console.Write("Enter quantity to add (negative to reduce): ");

        if (!int.TryParse(Console.ReadLine(), out var quantity))
        {
            Console.WriteLine("❌ Invalid quantity.");
            return;
        }

        if (quantity > 0)
        {
            product.IncreaseStock(quantity);
            await _productRepository.UpdateProductAsync(product);
            Console.WriteLine($"✓ Stock increased. New stock: {product.Stock} units");
        }
        else if (quantity < 0)
        {
            var decreaseQty = new Quantity(Math.Abs(quantity));
            product.DecreaseStock(decreaseQty);
            await _productRepository.UpdateProductAsync(product);
            Console.WriteLine($"✓ Stock decreased. New stock: {product.Stock} units");
        }
        else
        {
            Console.WriteLine("⚠ No changes made.");
        }
    }

    private async Task ToggleProductStatusAsync()
    {
        Console.Write("\nEnter Product ID: ");
        var idInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(idInput) || !Guid.TryParse(idInput, out var id))
        {
            Console.WriteLine("❌ Invalid Product ID.");
            return;
        }

        var product = await _productRepository.GetProductByIdAsync(new ProductId(id));
        if (product == null)
        {
            Console.WriteLine("❌ Product not found.");
            return;
        }

        if (product.IsActive)
        {
            product.Deactivate();
            Console.WriteLine($"✓ Product '{product.Name}' deactivated.");
        }
        else
        {
            product.Activate();
            Console.WriteLine($"✓ Product '{product.Name}' activated.");
        }

        await _productRepository.UpdateProductAsync(product);
    }

    // ============================================
    // CUSTOMER MANAGEMENT
    // ============================================
    private async Task CustomerManagementMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║      👤 CUSTOMER MANAGEMENT            ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");
            Console.WriteLine("1. Register New Customer");
            Console.WriteLine("2. List All Customers");
            Console.WriteLine("3. View Customer Details");
            Console.WriteLine("4. Login as Customer");
            Console.WriteLine("5. Logout");
            Console.WriteLine("6. Activate/Deactivate Customer");
            Console.WriteLine("7. Back to Main Menu");
            Console.Write("\nEnter your choice: ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    await RegisterCustomerAsync();
                    break;
                case "2":
                    await ListAllCustomersAsync();
                    break;
                case "3":
                    await ViewCustomerDetailsAsync();
                    break;
                case "4":
                    await LoginAsCustomerAsync();
                    break;
                case "5":
                    LogoutCustomer();
                    break;
                case "6":
                    await ToggleCustomerStatusAsync();
                    break;
                case "7":
                    return;
                default:
                    Console.WriteLine("\n❌ Invalid choice.");
                    break;
            }

            if (choice != "7")
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private async Task RegisterCustomerAsync()
    {
        Console.WriteLine("\n➕ REGISTER NEW CUSTOMER");
        Console.WriteLine("═════════════════════════════════════");

        Console.Write("Full Name: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("❌ Name cannot be empty.");
            return;
        }

        Console.Write("Email: ");
        var email = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            Console.WriteLine("❌ Invalid email address.");
            return;
        }

        var customer = Customer.Create(name, email);
        await _customerRepository.AddCustomerAsync(customer);

        Console.WriteLine($"\n✓ Customer '{name}' registered successfully!");
        Console.WriteLine($"  Customer ID: {customer.Id}");
    }

    private async Task ListAllCustomersAsync()
    {
        var customers = await _customerRepository.GetAllCustomersAsync();
        Console.WriteLine("\n👥 CUSTOMER LIST");
        Console.WriteLine("═════════════════════════════════════════════════════════════════════");
        Console.WriteLine($"{"ID",-38} {"Name",-25} {"Email",-30} {"Status"}");
        Console.WriteLine("─────────────────────────────────────────────────────────────────────");

        foreach (var customer in customers)
        {
            var status = customer.IsActive ? "✓ Active" : "✗ Inactive";
            Console.WriteLine($"{customer.Id.ToString(),-38} {TruncateString(customer.Name, 23),-25} {TruncateString(customer.Email, 28),-30} {status}");
        }

        Console.WriteLine("═════════════════════════════════════════════════════════════════════");
        Console.WriteLine($"Total Customers: {customers.Count()}");
    }

    private async Task ViewCustomerDetailsAsync()
    {
        Console.Write("\nEnter Customer ID: ");
        var idInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(idInput) || !Guid.TryParse(idInput, out var id))
        {
            Console.WriteLine("❌ Invalid Customer ID.");
            return;
        }

        var customer = await _customerRepository.GetCustomerByIdAsync(new CustomerId(id));
        if (customer == null)
        {
            Console.WriteLine("❌ Customer not found.");
            return;
        }

        var orders = await _orderRepository.GetOrdersByCustomerIdAsync(customer.Id.Value);

        Console.WriteLine("\n👤 CUSTOMER DETAILS");
        Console.WriteLine("═════════════════════════════════════");
        Console.WriteLine($"ID:           {customer.Id}");
        Console.WriteLine($"Name:         {customer.Name}");
        Console.WriteLine($"Email:        {customer.Email}");
        Console.WriteLine($"Status:       {(customer.IsActive ? "✓ Active" : "✗ Inactive")}");
        Console.WriteLine($"Cart Items:   {customer.Cart.Items.Count}");
        Console.WriteLine($"Total Orders: {orders.Count()}");
        Console.WriteLine("═════════════════════════════════════");
    }

    private async Task LoginAsCustomerAsync()
    {
        Console.Write("\nEnter Customer ID: ");
        var idInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(idInput) || !Guid.TryParse(idInput, out var id))
        {
            Console.WriteLine("❌ Invalid Customer ID.");
            return;
        }

        var customer = await _customerRepository.GetCustomerByIdAsync(new CustomerId(id));
        if (customer == null)
        {
            Console.WriteLine("❌ Customer not found.");
            return;
        }

        if (!customer.IsActive)
        {
            Console.WriteLine("❌ This customer account is inactive.");
            return;
        }

        _currentCustomer = customer;
        Console.WriteLine($"\n✓ Logged in as {customer.Name}");
    }

    private void LogoutCustomer()
    {
        if (_currentCustomer == null)
        {
            Console.WriteLine("\n⚠ No customer is currently logged in.");
            return;
        }

        Console.WriteLine($"\n✓ Logged out from {_currentCustomer.Name}");
        _currentCustomer = null;
    }

    private async Task ToggleCustomerStatusAsync()
    {
        Console.Write("\nEnter Customer ID: ");
        var idInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(idInput) || !Guid.TryParse(idInput, out var id))
        {
            Console.WriteLine("❌ Invalid Customer ID.");
            return;
        }

        var customer = await _customerRepository.GetCustomerByIdAsync(new CustomerId(id));
        if (customer == null)
        {
            Console.WriteLine("❌ Customer not found.");
            return;
        }

        if (customer.IsActive)
        {
            customer.Deactivate();
            Console.WriteLine($"✓ Customer '{customer.Name}' deactivated.");
            if (_currentCustomer?.Id == customer.Id)
            {
                _currentCustomer = null;
                Console.WriteLine("  You have been logged out.");
            }
        }
        else
        {
            customer.Activate();
            Console.WriteLine($"✓ Customer '{customer.Name}' activated.");
        }

        await _customerRepository.UpdateCustomerAsync(customer);
    }

    // ============================================
    // SHOPPING
    // ============================================
    private async Task ShoppingMenuAsync()
    {
        if (_currentCustomer == null)
        {
            Console.WriteLine("\n❌ Please login as a customer first!");
            Console.WriteLine("   Go to Customer Management → Login as Customer");
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║         🛍️  SHOPPING                    ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine($"Customer: {_currentCustomer.Name}\n");
            Console.WriteLine("1. Browse Products");
            Console.WriteLine("2. Add Product to Cart");
            Console.WriteLine("3. View Cart");
            Console.WriteLine("4. Remove from Cart");
            Console.WriteLine("5. Checkout");
            Console.WriteLine("6. Back to Main Menu");
            Console.Write("\nEnter your choice: ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    await BrowseProductsAsync();
                    break;
                case "2":
                    await AddToCartAsync();
                    break;
                case "3":
                    await ViewCartAsync();
                    break;
                case "4":
                    await RemoveFromCartAsync();
                    break;
                case "5":
                    await CheckoutAsync();
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("\n❌ Invalid choice.");
                    break;
            }

            if (choice != "6")
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private async Task BrowseProductsAsync()
    {
        var products = await _productRepository.GetAllProductsAsync();
        var activeProducts = products.Where(p => p.IsActive && p.Stock > 0);

        Console.WriteLine("\n🛍️  AVAILABLE PRODUCTS");
        Console.WriteLine("═════════════════════════════════════════════════════════════");
        Console.WriteLine($"{"ID",-38} {"Name",-25} {"Price",-12} {"Stock"}");
        Console.WriteLine("─────────────────────────────────────────────────────────────");

        foreach (var product in activeProducts)
        {
            Console.WriteLine($"{product.Id.ToString(),-38} {TruncateString(product.Name, 23),-25} {product.Price.ToString(),-12} {product.Stock}");
        }

        Console.WriteLine("═════════════════════════════════════════════════════════════");
        Console.WriteLine($"Total Available Products: {activeProducts.Count()}");
    }

    private async Task AddToCartAsync()
    {
        Console.Write("\nEnter Product ID: ");
        var idInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(idInput) || !Guid.TryParse(idInput, out var id))
        {
            Console.WriteLine("❌ Invalid Product ID.");
            return;
        }

        var product = await _productRepository.GetProductByIdAsync(new ProductId(id));
        if (product == null)
        {
            Console.WriteLine("❌ Product not found.");
            return;
        }

        if (!product.IsActive)
        {
            Console.WriteLine("❌ This product is not available.");
            return;
        }

        Console.Write($"Enter quantity (Available: {product.Stock}): ");
        if (!int.TryParse(Console.ReadLine(), out var qty) || qty <= 0)
        {
            Console.WriteLine("❌ Invalid quantity.");
            return;
        }

        if (qty > product.Stock)
        {
            Console.WriteLine($"❌ Insufficient stock. Only {product.Stock} units available.");
            return;
        }

        var quantity = new Quantity(qty);
        _currentCustomer!.Cart.AddItem(product, quantity);
        await _customerRepository.UpdateCustomerAsync(_currentCustomer);

        Console.WriteLine($"\n✓ Added {qty}x '{product.Name}' to cart!");
    }

    private async Task ViewCartAsync()
    {
        if (_currentCustomer!.Cart.IsEmpty)
        {
            Console.WriteLine("\n🛒 Your cart is empty.");
            return;
        }

        Console.WriteLine("\n🛒 YOUR CART");
        Console.WriteLine("═════════════════════════════════════════════════════════════════");
        Console.WriteLine($"{"Product ID",-38} {"Name",-20} {"Qty",-5} {"Unit Price",-12} {"Subtotal"}");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");

        foreach (var item in _currentCustomer.Cart.Items)
        {
            Console.WriteLine($"{item.ProductId.ToString(),-38} {TruncateString(item.ProductName, 18),-20} {item.Quantity.Value,-5} {item.UnitPrice.ToString(),-12} {item.TotalPrice}");
        }

        Console.WriteLine("═════════════════════════════════════════════════════════════════");
        Console.WriteLine($"Total: {_currentCustomer.Cart.TotalAmount}");
        Console.WriteLine($"Items: {_currentCustomer.Cart.Items.Count}");
    }

    private async Task RemoveFromCartAsync()
    {
        if (_currentCustomer!.Cart.IsEmpty)
        {
            Console.WriteLine("\n🛒 Your cart is empty.");
            return;
        }

        await ViewCartAsync();

        Console.Write("\nEnter Product ID to remove: ");
        var idInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(idInput) || !Guid.TryParse(idInput, out var id))
        {
            Console.WriteLine("❌ Invalid Product ID.");
            return;
        }

        _currentCustomer.Cart.RemoveItem(new ProductId(id));
        await _customerRepository.UpdateCustomerAsync(_currentCustomer);

        Console.WriteLine("\n✓ Item removed from cart!");
    }

    private async Task CheckoutAsync()
    {
        if (_currentCustomer!.Cart.IsEmpty)
        {
            Console.WriteLine("\n❌ Your cart is empty. Add some products first!");
            return;
        }

        await ViewCartAsync();

        Console.Write("\n\nProceed with checkout? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();

        if (confirm != "y")
        {
            Console.WriteLine("❌ Checkout cancelled.");
            return;
        }

        Console.WriteLine("\n⏳ Processing payment...");
        await Task.Delay(1000); // Simulate processing

        var orderId = await _checkoutService.CheckoutAsync(_currentCustomer.Id);

        // Refresh customer data
        _currentCustomer = await _customerRepository.GetCustomerByIdAsync(_currentCustomer.Id);

        Console.WriteLine("\n✅ ORDER SUCCESSFUL!");
        Console.WriteLine("═════════════════════════════════════");
        Console.WriteLine($"Order ID: {orderId}");
        Console.WriteLine($"Status:   Paid");
        Console.WriteLine("═════════════════════════════════════");
        Console.WriteLine("\n🎉 Thank you for your purchase!");
    }

    // ============================================
    // ORDER MANAGEMENT
    // ============================================
    private async Task OrderManagementMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║       📋 ORDER MANAGEMENT              ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");
            Console.WriteLine("1. View All Orders");
            Console.WriteLine("2. View Customer Orders");
            Console.WriteLine("3. View Order Details");
            Console.WriteLine("4. Cancel Order");
            Console.WriteLine("5. Back to Main Menu");
            Console.Write("\nEnter your choice: ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    await ViewAllOrdersAsync();
                    break;
                case "2":
                    await ViewCustomerOrdersAsync();
                    break;
                case "3":
                    await ViewOrderDetailsAsync();
                    break;
                case "4":
                    await CancelOrderAsync();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("\n❌ Invalid choice.");
                    break;
            }

            if (choice != "5")
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private async Task ViewAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllOrdersAsync();

        if (!orders.Any())
        {
            Console.WriteLine("\n📋 No orders found.");
            return;
        }

        Console.WriteLine("\n📋 ALL ORDERS");
        Console.WriteLine("═════════════════════════════════════════════════════════════════════════");
        Console.WriteLine($"{"Order ID",-38} {"Customer ID",-38} {"Total",-15} {"Status"}");
        Console.WriteLine("─────────────────────────────────────────────────────────────────────────");

        foreach (var order in orders)
        {
            Console.WriteLine($"{order.Id.ToString(),-38} {order.CustomerId.ToString(),-38} {order.TotalAmount.ToString(),-15} {order.Status}");
        }

        Console.WriteLine("═════════════════════════════════════════════════════════════════════════");
        Console.WriteLine($"Total Orders: {orders.Count()}");
    }

    private async Task ViewCustomerOrdersAsync()
    {
        Console.Write("\nEnter Customer ID: ");
        var idInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(idInput) || !Guid.TryParse(idInput, out var customerId))
        {
            Console.WriteLine("❌ Invalid Customer ID.");
            return;
        }

        var customer = await _customerRepository.GetCustomerByIdAsync(new CustomerId(customerId));
        if (customer == null)
        {
            Console.WriteLine("❌ Customer not found.");
            return;
        }

        var orders = await _orderRepository.GetOrdersByCustomerIdAsync(customerId);

        if (!orders.Any())
        {
            Console.WriteLine($"\n📋 No orders found for customer '{customer.Name}'.");
            return;
        }

        Console.WriteLine($"\n📋 ORDERS FOR: {customer.Name}");
        Console.WriteLine("═════════════════════════════════════════════════════════════");
        Console.WriteLine($"{"Order ID",-38} {"Total",-15} {"Status"}");
        Console.WriteLine("─────────────────────────────────────────────────────────────");

        foreach (var order in orders)
        {
            Console.WriteLine($"{order.Id.ToString(),-38} {order.TotalAmount.ToString(),-15} {order.Status}");
        }

        Console.WriteLine("═════════════════════════════════════════════════════════════");
        Console.WriteLine($"Total Orders: {orders.Count()}");
    }

    private async Task ViewOrderDetailsAsync()
    {
        Console.Write("\nEnter Order ID: ");
        var idInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(idInput) || !Guid.TryParse(idInput, out var id))
        {
            Console.WriteLine("❌ Invalid Order ID.");
            return;
        }

        var order = await _orderRepository.GetOrderByIdAsync(new OrderId(id));
        if (order == null)
        {
            Console.WriteLine("❌ Order not found.");
            return;
        }

        var customer = await _customerRepository.GetCustomerByIdAsync(new CustomerId(order.CustomerId));

        Console.WriteLine("\n📋 ORDER DETAILS");
        Console.WriteLine("═════════════════════════════════════════════════════════════");
        Console.WriteLine($"Order ID:     {order.Id}");
        Console.WriteLine($"Customer:     {customer?.Name ?? "Unknown"} ({order.CustomerId})");
        Console.WriteLine($"Status:       {order.Status}");
        Console.WriteLine($"Total Amount: {order.TotalAmount}");
        Console.WriteLine("\nOrder Items:");
        Console.WriteLine("─────────────────────────────────────────────────────────────");
        Console.WriteLine($"{"Product",-30} {"Qty",-5} {"Price",-12} {"Subtotal"}");
        Console.WriteLine("─────────────────────────────────────────────────────────────");

        foreach (var item in order.Items)
        {
            Console.WriteLine($"{TruncateString(item.ProductName, 28),-30} {item.Quantity.Value,-5} {item.Price.ToString(),-12} {item.TotalPrice}");
        }

        Console.WriteLine("═════════════════════════════════════════════════════════════");
    }

    private async Task CancelOrderAsync()
    {
        Console.Write("\nEnter Order ID: ");
        var idInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(idInput) || !Guid.TryParse(idInput, out var id))
        {
            Console.WriteLine("❌ Invalid Order ID.");
            return;
        }

        var order = await _orderRepository.GetOrderByIdAsync(new OrderId(id));
        if (order == null)
        {
            Console.WriteLine("❌ Order not found.");
            return;
        }

        if (order.Status != OrderStatus.Pending)
        {
            Console.WriteLine($"❌ Cannot cancel order with status: {order.Status}");
            return;
        }

        order.Cancel();
        await _orderRepository.UpdateOrderAsync(order);

        Console.WriteLine($"\n✓ Order {order.Id} has been cancelled.");
    }

    // ============================================
    // UTILITY METHODS
    // ============================================
    private string TruncateString(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return value.Length <= maxLength ? value : value.Substring(0, maxLength - 2) + "..";
    }
}


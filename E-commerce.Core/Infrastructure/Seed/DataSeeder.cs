public static class DataSeeder
{
    public static async Task SeedDataAsync(
        ICustomerRepository customerRepository,
        IProductRepository productRepository)
    {
        // Seed Products
        var products = new[]
        {
            Product.Create("Laptop Dell XPS 15", new Money(1299.99m, "USD"), 10),
            Product.Create("iPhone 14 Pro", new Money(999.99m, "USD"), 25),
            Product.Create("Samsung Galaxy S23", new Money(899.99m, "USD"), 20),
            Product.Create("iPad Air", new Money(599.99m, "USD"), 15),
            Product.Create("MacBook Pro M3", new Money(2499.99m, "USD"), 8),
            Product.Create("Sony WH-1000XM5 Headphones", new Money(399.99m, "USD"), 30),
            Product.Create("Logitech MX Master 3 Mouse", new Money(99.99m, "USD"), 50),
            Product.Create("Mechanical Keyboard RGB", new Money(149.99m, "USD"), 40),
            Product.Create("4K Monitor 27 inch", new Money(449.99m, "USD"), 12),
            Product.Create("USB-C Hub", new Money(49.99m, "USD"), 60),
        };

        foreach (var product in products)
        {
            await productRepository.AddProductAsync(product);
        }

        // Seed Customers
        var customers = new[]
        {
            Customer.Create("John Doe", "john.doe@email.com"),
            Customer.Create("Jane Smith", "jane.smith@email.com"),
            Customer.Create("Bob Johnson", "bob.johnson@email.com"),
            Customer.Create("Alice Williams", "alice.williams@email.com"),
            Customer.Create("Charlie Brown", "charlie.brown@email.com"),
        };

        foreach (var customer in customers)
        {
            await customerRepository.AddCustomerAsync(customer);
        }

        Console.WriteLine("✓ Sample data seeded successfully!");
        Console.WriteLine($"  - {products.Length} products added");
        Console.WriteLine($"  - {customers.Length} customers registered");
    }
}

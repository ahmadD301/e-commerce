
public class Product
{
    public ProductId Id { get; private set; }
    public string Name { get; private set; }
    public Money Price { get; private set; }
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }

    private Product()
    {
        // Required by EF Core
        Name = null!;
        Price = null!;
    }

    private Product(
        ProductId id,
        string name,
        Money price,
        int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.", nameof(name));

        if (stock < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(stock));

        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
        IsActive = true; // default active
    }

    public static Product Create(
        string name,
        Money price,
        int initialStock)
    {
        return new Product(
            new ProductId(Guid.NewGuid()),
            name,
            price,
            initialStock);
    }

    public void ChangePrice(Money newPrice)
    {
        Price = newPrice;
    }

    public void IncreaseStock(int quantity)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot increase stock of inactive product.");
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        Stock += quantity;
    }

    public void DecreaseStock(Quantity quantity)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot decrease stock of inactive product.");

        if (Stock < quantity)
            throw new InvalidOperationException("Insufficient stock.");

        Stock -= quantity;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public bool IsInStock(Quantity quantity)
        => IsActive && Stock >= quantity;
}
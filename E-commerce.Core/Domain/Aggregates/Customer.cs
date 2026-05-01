public class Customer
{
    public CustomerId Id { get; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public Cart Cart { get;  }
    public bool IsActive { get; private set; }
    private Customer()
    {
        Name = string.Empty;
        Email = string.Empty;
        Cart = null!;
    }
    public Customer(CustomerId id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
        Cart = new Cart();
        IsActive = true;
    }
    public static Customer Create(string name, string email)
    {
        var id = new CustomerId(Guid.NewGuid());
        return new Customer(id, name, email);
    }
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void checkout()
    {
        if( Cart.IsEmpty)
            throw new InvalidOperationException("Cannot checkout an empty cart.");
        if (!IsActive)
            throw new InvalidOperationException("Cannot checkout with an inactive customer.");
        
        // Application Service will handle:
        // 1. Order creation
        // 2. Payment creation
        // 3. Stock reduction
        // Cart will be cleared after successful checkout
        System.Console.WriteLine("Checkout process initiated for customer: " + Name);
    }

}
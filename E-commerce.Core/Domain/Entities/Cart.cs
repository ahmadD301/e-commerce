

public class Cart
{
    private readonly List<CartItem> _items = new List<CartItem>();
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();
    public bool IsEmpty => !_items.Any();

    internal Cart() { }
    
    public void AddItem(Product product, Quantity quantity)
    {
        if (!product.IsActive)
            throw new ArgumentNullException(nameof(product), "Cannot add inactive product to cart.");

        var item = new CartItem(product.Id, product.Name, product.Price, quantity); 
        var existingItem = _items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(item.Quantity);
        }
        else
        {
            _items.Add(item);
        }
    }

    public void RemoveItem(ProductId productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _items.Remove(item);
        }else
        {
            throw new ArgumentException("Product not found in cart.", nameof(productId));
        }
    }
    public void Clear()
    {
        _items.Clear();
    }
    public Money TotalAmount => _items.Aggregate(Money.Zero("USD"), (sum, item) => sum.Add(item.TotalPrice));
}

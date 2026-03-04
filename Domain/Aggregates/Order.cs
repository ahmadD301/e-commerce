


public class Order
{
    private readonly List<OrderItem> _items = new List<OrderItem>();
    public OrderId Id {get;}
    public Guid CustomerId { get; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public OrderStatus Status { get; private set; }
    public Money TotalAmount => CalculateTotalAmount();

    public Order(OrderId id, Guid customerId , IEnumerable<OrderItem> items) 
    {

        Id = id;
        CustomerId = customerId;
        _items.AddRange(items);
        Status = OrderStatus.Pending;
        if (!Items.Any())
        {
            throw new ArgumentException("Order must contain at least one item.");
        }
    }
    public static Order Create(Guid customerId, IEnumerable<OrderItem> items)
    {
        return new Order(new OrderId(Guid.NewGuid()), customerId, items);
    }

    public Money CalculateTotalAmount()
    {
        return _items
            .Select(i => i.TotalPrice)
            .Aggregate((acc, next) => acc.Add(next));
    }
    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be marked as paid.");

        Status = OrderStatus.Paid;
    }
    public void Cancel()
    {
        if (Status == OrderStatus.Paid)
            throw new InvalidOperationException("Cannot cancel a paid order.");

        Status = OrderStatus.Cancelled;
    }
    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Can only add items to pending orders.");

        _items.Add(item);
    }
    public void RemoveItem(ProductId productId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Can only remove items from pending orders.");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new ArgumentException("Item not found in order.", nameof(productId));

        _items.Remove(item);
    }
}
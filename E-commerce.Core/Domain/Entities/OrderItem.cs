

public class OrderItem
{
    public ProductId ProductId { get;}
    public string  ProductName { get; }
    public Money Price { get; }
    public Quantity Quantity { get; }
    public Money TotalPrice => Price.Multiply(Quantity.Value);

    public OrderItem(ProductId productId, string productName, Money price, Quantity quantity)
    {
        ProductId = productId;
        ProductName = productName;
        Price = price;
        Quantity = quantity;
    }

}
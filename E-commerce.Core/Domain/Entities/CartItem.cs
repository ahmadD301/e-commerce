


public class CartItem
{
    public ProductId ProductId { get;}
    public string ProductName { get; }
    public Money UnitPrice { get; }
    public Quantity Quantity { get; private set; }
    public Money TotalPrice => UnitPrice.Multiply(Quantity);
    private CartItem()
    {
        ProductName = string.Empty;
        UnitPrice = null!;
    }
    public CartItem(ProductId productId, string productName, Money unitPrice, Quantity quantity)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void IncreaseQuantity(Quantity quantity)
    {
        Quantity = new Quantity(Quantity.Value + quantity.Value);
    }
    public void DecreaseQuantity(Quantity quantity)
    {
        Quantity = new Quantity(Quantity.Value - quantity.Value);
    }
    
}
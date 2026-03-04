
// record mean immutable and value-based equality, struct mean value type (no heap allocation)
public readonly record struct CustomerId(Guid Value) 
{
    public static CustomerId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
public readonly record struct ProductId(Guid Value) 
{
    public static ProductId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct OrderId(Guid Value) 
{
    public static OrderId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
public readonly record struct PaymentId(Guid Value)
{
    public static PaymentId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
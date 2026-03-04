

public readonly record struct Quantity
{
    private const int MaxQuantity = 1000; // Maximum limit

    public int Value { get; }// Quantity value
    public Quantity(int value)
    {
        if (value < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(value));
        if (value > MaxQuantity)
            throw new ArgumentException($"Quantity cannot exceed {MaxQuantity}.", nameof(value));

        Value = value;
    }
    public static Quantity of(int value) => new(value);// Factory method for creating Quantity instances
    public override string ToString() => Value.ToString();

    public static implicit operator int(Quantity quantity) => quantity.Value;// Implicit conversion to int for easy usage
}
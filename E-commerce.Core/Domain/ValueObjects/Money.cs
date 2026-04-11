
public readonly record struct Money
{
    public decimal Amount { get;}
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or whitespace.", nameof(currency));
        
        Amount = amount;
        Currency = currency;
    }

    public static Money Zero(string currency) => new Money(0, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        
        return new Money(this.Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);

        if (this.Amount < other.Amount)
            throw new InvalidOperationException("Resulting amount cannot be negative.");

        return new Money(this.Amount - other.Amount, Currency);
    }
    public Money Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("Factor cannot be negative.", nameof(factor));

        return new Money(this.Amount * factor, Currency);
    }

    public void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Money values must have the same currency.");
    }
    public override string ToString() => $"{Amount} {Currency}";
}
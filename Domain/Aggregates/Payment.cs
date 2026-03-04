

public class Payment
{
    public PaymentId Id {get;}
    public OrderId OrderId { get; }
    public Money Amount { get; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; }     

    public Payment(PaymentId id, OrderId orderId, Money amount)
    {
        Id = id;
        OrderId = orderId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static Payment Create(Order order)
    {
        if(order.Status != OrderStatus.Paid)
            throw new InvalidOperationException("Payment can only be created for paid orders.");
        
        return new Payment(new PaymentId(Guid.NewGuid()), order.Id, order.TotalAmount);
    }

    public void MarkAsCompleted()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can be marked as completed.");

        Status = PaymentStatus.Completed;
    }
    public void MarkAsFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can be marked as failed.");

        Status = PaymentStatus.Failed;
    }
    public void Cancel()
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed payment.");

        Status = PaymentStatus.Cancelled;
    }

    public bool IsCompleted => Status == PaymentStatus.Completed;
}
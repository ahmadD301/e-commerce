using Microsoft.EntityFrameworkCore;
using Ecommerce.Infrastructure.Data;

namespace Ecommerce.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _dbContext;

    public PaymentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Payment?> GetPaymentByIdAsync(PaymentId id)
    {
        return await _dbContext.Payments
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        return await _dbContext.Payments.ToListAsync();
    }

    public async Task AddPaymentAsync(Payment payment)
    {
        await _dbContext.Payments.AddAsync(payment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePaymentAsync(Payment payment)
    {
        _dbContext.Payments.Update(payment);
        await _dbContext.SaveChangesAsync();
    }
}

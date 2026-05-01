using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ecommerce.Infrastructure.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
	{
	}

	public DbSet<Product> Products => Set<Product>();
	public DbSet<Customer> Customers => Set<Customer>();
	public DbSet<Order> Orders => Set<Order>();
	public DbSet<Payment> Payments => Set<Payment>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		var customerIdConverter = new ValueConverter<CustomerId, Guid>(
			id => id.Value,
			value => new CustomerId(value));

		var productIdConverter = new ValueConverter<ProductId, Guid>(
			id => id.Value,
			value => new ProductId(value));

		var orderIdConverter = new ValueConverter<OrderId, Guid>(
			id => id.Value,
			value => new OrderId(value));

		var paymentIdConverter = new ValueConverter<PaymentId, Guid>(
			id => id.Value,
			value => new PaymentId(value));

		var quantityConverter = new ValueConverter<Quantity, int>(
			quantity => quantity.Value,
			value => new Quantity(value));

		var product = modelBuilder.Entity(typeof(Product));
		product.ToTable("Products");
		product.HasKey("Id");
		product.Property("Id")
			.HasConversion(productIdConverter)
			.ValueGeneratedNever();
		product.Property("Name").IsRequired();
		product.Property("Stock").IsRequired();
		product.Property("IsActive").IsRequired();
		product.OwnsOne(typeof(Money), "Price", money =>
		{
			money.Property("Amount")
				.HasColumnName("PriceAmount")
				.IsRequired();
			money.Property("Currency")
				.HasColumnName("PriceCurrency")
				.HasMaxLength(8)
				.IsRequired();
		});

		var customer = modelBuilder.Entity(typeof(Customer));
		customer.ToTable("Customers");
		customer.HasKey("Id");
		customer.Property("Id")
			.HasConversion(customerIdConverter)
			.ValueGeneratedNever();
		customer.Property("Name").IsRequired();
		customer.Property("Email").IsRequired();
		customer.Property("IsActive").IsRequired();
		customer.OwnsOne(typeof(Cart), "Cart", cart =>
		{
			cart.ToTable("Carts");
			cart.HasKey("CustomerId");
			cart.WithOwner().HasForeignKey("CustomerId");
			cart.Ignore("IsEmpty");
			cart.Ignore("TotalAmount");
			cart.OwnsMany(typeof(CartItem), "Items", item =>
			{
				item.ToTable("CartItems");
				item.Property<int>("Id").ValueGeneratedOnAdd();
				item.HasKey("Id");
				item.WithOwner();
				item.Property("ProductId")
					.HasConversion(productIdConverter)
					.IsRequired();
				item.Property("ProductName").IsRequired();
				item.Property("Quantity")
					.HasConversion(quantityConverter)
					.IsRequired();
				item.OwnsOne(typeof(Money), "UnitPrice", money =>
				{
					money.Property("Amount")
						.HasColumnName("UnitPriceAmount")
						.IsRequired();
					money.Property("Currency")
						.HasColumnName("UnitPriceCurrency")
						.HasMaxLength(8)
						.IsRequired();
				});
				item.Ignore("TotalPrice");
			});
			cart.Navigation("Items")
				.UsePropertyAccessMode(PropertyAccessMode.Field);
		});

		var order = modelBuilder.Entity(typeof(Order));
		order.ToTable("Orders");
		order.HasKey("Id");
		order.Property("Id")
			.HasConversion(orderIdConverter)
			.ValueGeneratedNever();
		order.Property("CustomerId").IsRequired();
		order.Property("Status").IsRequired();
		order.Ignore("TotalAmount");
		order.OwnsMany(typeof(OrderItem), "Items", item =>
		{
			item.ToTable("OrderItems");
			item.Property<int>("Id").ValueGeneratedOnAdd();
			item.HasKey("Id");
			item.Property("OrderId")
				.HasConversion(orderIdConverter);
			item.WithOwner().HasForeignKey("OrderId");
			item.Property("ProductId")
				.HasConversion(productIdConverter)
				.IsRequired();
			item.Property("ProductName").IsRequired();
			item.Property("Quantity")
				.HasConversion(quantityConverter)
				.IsRequired();
			item.OwnsOne(typeof(Money), "Price", money =>
			{
				money.Property("Amount")
					.HasColumnName("PriceAmount")
					.IsRequired();
				money.Property("Currency")
					.HasColumnName("PriceCurrency")
					.HasMaxLength(8)
					.IsRequired();
			});
			item.Ignore("TotalPrice");
		});
		order.Navigation("Items")
			.UsePropertyAccessMode(PropertyAccessMode.Field);

		var payment = modelBuilder.Entity(typeof(Payment));
		payment.ToTable("Payments");
		payment.HasKey("Id");
		payment.Property("Id")
			.HasConversion(paymentIdConverter)
			.ValueGeneratedNever();
		payment.Property("OrderId")
			.HasConversion(orderIdConverter)
			.IsRequired();
		payment.Property("Status").IsRequired();
		payment.Property("CreatedAt").IsRequired();
		payment.Ignore("IsCompleted");
		payment.OwnsOne(typeof(Money), "Amount", money =>
		{
			money.Property("Amount")
				.HasColumnName("Amount")
				.IsRequired();
			money.Property("Currency")
				.HasColumnName("Currency")
				.HasMaxLength(8)
				.IsRequired();
		});
	}
}

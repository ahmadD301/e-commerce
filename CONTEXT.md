# CONTEXT

This document captures the current state of the Ecommerce solution as of May 1, 2026. It is descriptive only and reflects the existing codebase.

## Workspace Structure (Source/Config/Docs)

- Ecommerce.slnx
- README.md
- .gitignore
- E-commerce.Core/
  - E-commerce.Core.csproj
  - E-commerce.sln
  - Program.cs
  - FUTURE_WORK.md
  - Application/
    - Interfaces/
      - IRepository.cs
      - InMemoryRepository.cs
    - Services/
      - CheckoutService.cs
  - Domain/
    - Aggregates/
      - Customer.cs
      - Order.cs
      - Payment.cs
      - Product.cs
    - Entities/
      - Cart.cs
      - CartItem.cs
      - OrderItem.cs
    - Enums/
      - OrderStatus.cs
      - PaymentStatus.cs
    - ValueObjects/
      - Ids.cs
      - Money.cs
      - Quantity.cs
  - Infrastructure/
    - Seed/
      - DataSeeder.cs
- E-commerce.Test/
  - E-commerce.Test.csproj
  - UnitTest1.cs
  - Application/
    - Services/
      - CheckoutServiceTest.cs
  - Domain/
    - Aggregates/
      - CustomerTest.cs
      - OrderTest.cs
      - PaymentTest.cs
      - ProductTest.cs
    - Entities/
      - CartTest.cs
      - CartItemTest.cs
      - OrderItemTest.cs
- Ecommerce.Api/
  - Ecommerce.Api.csproj
  - Program.cs
  - Ecommerce.Api.http
  - appsettings.json
  - appsettings.Development.json
  - Properties/
    - launchSettings.json
- Ecommerce.Infrastructure/
  - Ecommerce.Infrastructure.csproj
  - Class1.cs
  - Data/
    - AppDbContext.cs

## Domain Models

### Aggregates

#### Customer
- Properties:
  - Id : CustomerId (get only)
  - Name : string (private set)
  - Email : string (private set)
  - Cart : Cart (get only)
  - IsActive : bool (private set)
- Construction:
  - Customer(CustomerId id, string name, string email)
  - static Create(string name, string email) -> Customer
- Behavior:
  - Deactivate()
  - Activate()
  - checkout() (lowercase method name)
    - Throws if cart is empty or customer is inactive; writes a console message.

#### Order
- Properties:
  - Id : OrderId (get only)
  - CustomerId : Guid (get only)
  - Items : IReadOnlyCollection<OrderItem> (read-only view over private list)
  - Status : OrderStatus (private set)
  - TotalAmount : Money (computed)
- Construction:
  - Order(OrderId id, Guid customerId, IEnumerable<OrderItem> items)
  - static Create(Guid customerId, IEnumerable<OrderItem> items) -> Order
- Behavior:
  - CalculateTotalAmount() -> Money (aggregate of item totals)
  - MarkAsPaid() (only when Pending)
  - Cancel() (not allowed if Paid)
  - AddItem(OrderItem item) (only when Pending)
  - RemoveItem(ProductId productId) (only when Pending; throws if not found)

#### Payment
- Properties:
  - Id : PaymentId (get only)
  - OrderId : OrderId (get only)
  - Amount : Money (get only)
  - Status : PaymentStatus (private set)
  - CreatedAt : DateTime (get only, UTC)
  - IsCompleted : bool (computed, Status == Completed)
- Construction:
  - Payment(PaymentId id, OrderId orderId, Money amount)
  - static Create(Order order) -> Payment (only if order is Pending)
- Behavior:
  - MarkAsCompleted() (only when Pending)
  - MarkAsFailed() (only when Pending)
  - Cancel() (not allowed if Completed)

#### Product
- Properties:
  - Id : ProductId (get only)
  - Name : string (private set)
  - Price : Money (private set)
  - Stock : int (private set)
  - IsActive : bool (private set)
- Construction:
  - private Product(ProductId id, string name, Money price, int stock)
  - static Create(string name, Money price, int initialStock) -> Product
- Behavior:
  - ChangePrice(Money newPrice)
  - IncreaseStock(int quantity) (only when active, quantity > 0)
  - DecreaseStock(Quantity quantity) (only when active and enough stock)
  - Deactivate()
  - Activate()
  - IsInStock(Quantity quantity) -> bool (active and stock >= qty)

### Entities

#### Cart
- Properties:
  - Items : IReadOnlyCollection<CartItem> (read-only view over private list)
  - IsEmpty : bool
  - TotalAmount : Money (computed by summing item totals in USD)
- Behavior:
  - AddItem(Product product, Quantity quantity)
    - Throws if product is inactive; merges quantities for same product.
  - RemoveItem(ProductId productId)
    - Throws if product not found in cart.
  - Clear()

#### CartItem
- Properties:
  - ProductId : ProductId (get only)
  - ProductName : string (get only)
  - UnitPrice : Money (get only)
  - Quantity : Quantity (private set)
  - TotalPrice : Money (computed)
- Behavior:
  - IncreaseQuantity(Quantity quantity)
  - DecreaseQuantity(Quantity quantity)

#### OrderItem
- Properties:
  - ProductId : ProductId (get only)
  - ProductName : string (get only)
  - Price : Money (get only)
  - Quantity : Quantity (get only)
  - TotalPrice : Money (computed)
- Behavior:
  - Constructor only (immutable line item)

## Value Objects

#### Ids (record structs)
- CustomerId(Guid Value)
- ProductId(Guid Value)
- OrderId(Guid Value)
- PaymentId(Guid Value)
- Common behavior:
  - static New() -> new id
  - ToString() returns Guid string

#### Money (record struct)
- Properties:
  - Amount : decimal (get only)
  - Currency : string (get only)
- Behavior:
  - static Zero(string currency)
  - Add(Money other) -> Money
  - Subtract(Money other) -> Money (throws if result negative)
  - Multiply(decimal factor) -> Money
  - EnsureSameCurrency(Money other) (throws if currency mismatch)
  - ToString() -> "{Amount} {Currency}"

#### Quantity (record struct)
- Properties:
  - Value : int (get only)
- Constraints:
  - 0 <= Value <= 1000
- Behavior:
  - static of(int value) -> Quantity
  - implicit conversion to int
  - ToString() -> Value as string

## Enums

- OrderStatus: Pending (0), Paid (1), Cancelled (2)
- PaymentStatus: Pending (0), Completed (1), Failed (2), Cancelled (3)

## Application Layer

### Interfaces

#### ICustomerRepository
- Task<Customer?> GetCustomerByIdAsync(CustomerId id)
- Task<IEnumerable<Customer>> GetAllCustomersAsync()
- Task AddCustomerAsync(Customer customer)
- Task UpdateCustomerAsync(Customer customer)

#### IProductRepository
- Task<Product?> GetProductByIdAsync(ProductId id)
- Task<IEnumerable<Product>> GetAllProductsAsync()
- Task AddProductAsync(Product product)
- Task UpdateProductAsync(Product product)

#### IOrderRepository
- Task<Order?> GetOrderByIdAsync(OrderId id)
- Task<IEnumerable<Order>> GetAllOrdersAsync()
- Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId)
- Task AddOrderAsync(Order order)
- Task UpdateOrderAsync(Order order)

#### IPaymentRepository
- Task<Payment?> GetPaymentByIdAsync(PaymentId id)
- Task<IEnumerable<Payment>> GetAllPaymentsAsync()
- Task AddPaymentAsync(Payment payment)
- Task UpdatePaymentAsync(Payment payment)

### In-memory Repository Implementations

- InMemoryCustomerRepository
- InMemoryProductRepository
- InMemoryOrderRepository
- InMemoryPaymentRepository

All implementations use in-memory Dictionary<TKey, TValue> storage and return Task-based results. Each provides GetById, GetAll, Add, Update methods, and order repository includes GetOrdersByCustomerIdAsync.

### Services

#### CheckoutService
- Dependencies: ICustomerRepository, IProductRepository, IOrderRepository, IPaymentRepository
- CheckoutAsync(CustomerId customerId) -> Task<OrderId>
  - Validates customer exists, active, cart not empty
  - For each CartItem: validates product exists and has stock
  - Creates Order from CartItems and saves it
  - Creates Payment for the Order and saves it
  - Simulates payment gateway (currently always succeeds)
  - On failure: marks payment failed, updates repo, throws
  - On success: marks payment completed and order paid
  - Decreases stock for each OrderItem
  - Clears customer cart and updates customer/order/payment repositories

## Console Application (E-commerce.Core/Program.cs)

The console application wires in-memory repositories and the CheckoutService, seeds data, and starts a CLI loop.

### ECommerceCLI
- Menus: Product Management, Customer Management, Shopping, Order Management, Exit
- Product management: list, view details, add, update stock, toggle active
- Customer management: register, list, view details, login/logout, toggle active
- Shopping: browse products, add to cart, view cart, remove from cart, checkout
- Order management: view all, view by customer, view details, cancel order

## Infrastructure Seed Data

DataSeeder.SeedDataAsync seeds:
- 10 products (electronics and accessories) with USD prices and initial stock
- 5 customers with name + email
- Writes a summary to console

## Ecommerce.Api (Current State)

- Minimal API template with WeatherForecast endpoint
- Adds OpenAPI services and maps OpenAPI only in Development
- HTTPS redirection is enabled
- No controllers or domain endpoints present

## Ecommerce.Infrastructure (Current State)

- AppDbContext.cs exists but is empty
- Class1.cs is empty
- Project references E-commerce.Core and includes EF Core SQL Server packages

## Tests (E-commerce.Test)

- Uses xUnit, Moq, AutoFixture; target net8.0
- UnitTest1 is empty placeholder

### CheckoutServiceTest
- Validates:
  - Success flow returns OrderId
  - Customer not found, inactive, empty cart
  - Missing product and out-of-stock errors
  - Payment failure handling
  - Stock reduction, cart clearing, and repository calls on success

### Domain Aggregate Tests
- CustomerTest: Create, Activate/Deactivate, checkout guard behavior
- OrderTest: create, total amount, status changes, add/remove item guards
- PaymentTest: creation rules, status transitions, IsCompleted
- ProductTest: creation guards, price change, stock adjustments, activate/deactivate

### Domain Entity Tests
- CartTest: add/remove/clear behavior and total calculation
- CartItemTest: creation and quantity changes
- OrderItemTest: creation and total price

## Existing vs. Not Present (Based on Current Repo)

### Present
- Domain models, value objects, enums
- Repository interfaces + in-memory implementations
- CheckoutService
- Console CLI application
- Unit tests for domain and checkout flow
- Minimal API template in Ecommerce.Api
- EF Core packages referenced in Ecommerce.Infrastructure

### Not Present in Code
- EF Core DbContext implementation and entity mappings
- EF Core repository implementations
- Database migrations
- API controllers or domain endpoints
- JWT authentication and authorization
- Swagger configuration beyond default OpenAPI in Development
- Dockerfile or container orchestration files

## Naming Conventions and Patterns

- PascalCase for types and members
- Async method names end with Async
- Value objects implemented as record structs
- Repository pattern with interfaces in Application/Interfaces
- Exceptions used for guard clauses and invalid state
- RootNamespace in E-commerce.Core and E-commerce.Test csproj is "_"
- One method deviates from PascalCase: Customer.checkout()

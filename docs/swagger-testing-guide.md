# Swagger Testing Guide

This guide walks through testing the API with Swagger, using seeded data and realistic request bodies.

## Seeded data you can use immediately

### Customers
- John Doe — john.doe@email.com
- Jane Smith — jane.smith@email.com
- Bob Johnson — bob.johnson@email.com
- Alice Williams — alice.williams@email.com
- Charlie Brown — charlie.brown@email.com

### Products
- Laptop Dell XPS 15 (1299.99 USD)
- iPhone 14 Pro (999.99 USD)
- Samsung Galaxy S23 (899.99 USD)
- iPad Air (599.99 USD)
- MacBook Pro M3 (2499.99 USD)
- Sony WH-1000XM5 Headphones (399.99 USD)
- Logitech MX Master 3 Mouse (99.99 USD)
- Mechanical Keyboard RGB (149.99 USD)
- 4K Monitor 27 inch (449.99 USD)
- USB-C Hub (49.99 USD)

## Step-by-step testing order

### 1) Auth (do this first)

**POST** http://localhost:5120/api/auth/login

Request body:
```json
{
  "email": "john.doe@email.com"
}
```

Copy from response:
- token
- customerId

Expected response:
- 200 OK

Note: Endpoints are not protected with [Authorize], so the token is optional. You can still paste it into Swagger's Authorize dialog as `Bearer <token>`.

### 2) Products

**GET** http://localhost:5120/api/products

Request body: none

Copy from response:
- any product id you want to use later

Expected response:
- 200 OK

**GET** http://localhost:5120/api/products/{id}

Request body: none

Expected response:
- 200 OK if found
- 404 Not Found if not found

**POST** http://localhost:5120/api/products

Request body:
```json
{
  "name": "Nintendo Switch OLED",
  "priceAmount": 349.99,
  "currency": "USD",
  "initialStock": 12
}
```

Copy from response:
- id (new product id)

Expected response:
- 201 Created

**PATCH** http://localhost:5120/api/products/{id}/stock

Request body:
```json
{
  "quantity": 5
}
```

Expected response:
- 204 No Content
- 400 Bad Request if quantity is zero or stock would go negative

**PATCH** http://localhost:5120/api/products/{id}/active

Request body:
```json
{
  "isActive": true
}
```

Expected response:
- 204 No Content

### 3) Customers

**GET** http://localhost:5120/api/customers

Request body: none

Copy from response:
- any customer id

Expected response:
- 200 OK

**GET** http://localhost:5120/api/customers/{id}

Request body: none

Expected response:
- 200 OK if found
- 404 Not Found if not found

**POST** http://localhost:5120/api/customers

Request body:
```json
{
  "name": "Michael Rivera",
  "email": "michael.rivera@email.com"
}
```

Copy from response:
- id (new customer id)

Expected response:
- 201 Created

### 4) Checkout

**POST** http://localhost:5120/api/checkout

Request body:
```json
{
  "customerId": "<customer-id-from-auth-or-customers>"
}
```

Copy from response:
- orderId

Expected response:
- 200 OK when the customer cart has items
- 400 Bad Request with message "Shopping cart is empty." when the cart is empty

Note: There is no API endpoint to add items to a cart in the current controllers. Checkout will fail unless the cart is populated by seeding or direct repository usage.

### 5) Orders

**GET** http://localhost:5120/api/orders

Request body: none

Copy from response:
- any order id

Expected response:
- 200 OK

**GET** http://localhost:5120/api/orders/{id}

Request body: none

Expected response:
- 200 OK if found
- 404 Not Found if not found

**GET** http://localhost:5120/api/orders/by-customer/{customerId}

Request body: none

Expected response:
- 200 OK

**POST** http://localhost:5120/api/orders/{id}/cancel

Request body: none

Expected response:
- 204 No Content
- 400 Bad Request if cancel is not allowed
- 404 Not Found if the order id does not exist

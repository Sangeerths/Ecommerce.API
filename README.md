# Ecommerce.API

A RESTful Web API for managing an e-commerce catalog — built with **ASP.NET Core** and **Entity Framework Core**. It exposes endpoints for managing **Products**, **Categories**, and **Sales**, with pagination and soft-delete support baked in.

## Features

- CRUD operations for Products, Categories, and Sales
- Paginated list endpoints (`PageNumber`, `PageSize`)
- Soft delete (records are flagged `IsDeleted` / `DeletedAt` rather than removed)
- DTO-based request/response separation to keep the API contract independent of the data model
- EF Core for data access

## Tech Stack

- **.NET / ASP.NET Core Web API**
- **Entity Framework Core** (`EcommerceDbContext`)
- SQL database (via EF Core provider — see `appsettings.json` for connection string)

## Project Structure

```
Ecommerce.API/
├── Controllers/
│   ├── ProductController.cs
│   ├── CategoryController.cs
│   └── SaleController.cs
├── Services/
│   ├── ProductService.cs
│   ├── CategoryService.cs
│   └── SaleService.cs
├── DTO/
│   ├── Product/
│   ├── Category/
│   ├── Sale/
│   └── Pagination/
├── Models/
│   ├── Product.cs
│   ├── Category.cs
│   └── Sale.cs
├── Data/
│   └── EcommerceDbContext.cs
└── Program.cs
```

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 
- A SQL Server

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/Sangeerths/Ecommerce.API.git
   cd Ecommerce.API
   ```

2. Configure your database connection string in `appsettings.json` (or `appsettings.Development.json`):
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "<your-connection-string>"
     }
   }
   ```

3. Apply EF Core migrations:
   ```bash
   dotnet ef database update
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

5. The API will be available at `https://localhost:5001` (or the port configured in `launchSettings.json`).

## API Endpoints

### Product (`/api/Product`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Product?PageNumber=1&PageSize=10` | Get all products (paginated) |
| GET | `/api/Product/{productId}` | Get a product by ID |
| POST | `/api/Product` | Create a new product |
| PUT | `/api/Product/{productId}` | Update an existing product |
| DELETE | `/api/Product/{productId}` | Soft-delete a product |

### Category (`/api/Category`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Category?PageNumber=1&PageSize=10` | Get all categories (paginated) |
| GET | `/api/Category/{categoryId}` | Get a category by ID |
| POST | `/api/Category` | Create a new category |
| PATCH | `/api/Category/{categoryId}` | Update an existing category |
| DELETE | `/api/Category/{categoryId}` | Soft-delete a category |

### Sale (`/api/Sale`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Sale?PageNumber=1&PageSize=10` | Get all sales (paginated) |
| GET | `/api/Sale/{saleId}` | Get a sale by ID |
| POST | `/api/Sale` | Create a new sale |
| PUT | `/api/Sale/{saleId}` | Update an existing sale |
| DELETE | `/api/Sale/{saleId}` | Soft-delete a sale |

## Request/Response Models

- **Product**: `name`, `description`, `price`, `stockQuantity`, `categoryId`
- **Category**: `name`, `description`
- **Sale**: `productId`, `quantity`, `unitPrice`, `customerName` (server computes `totalAmount` / `saleDate`)

All list endpoints return a `PagedResponse<T>` containing the items plus pagination metadata (page number, page size, total records).

## Testing the API

A Postman collection (`Ecommerce.API.postman_collection.json`) covering all endpoints is included in the repo. Import it into Postman and set the `base_url` variable to match your running instance to get started quickly.

## Soft Delete Behavior

Delete operations across all resources are non-destructive: records are flagged with `IsDeleted = true` and `DeletedAt` is set, rather than being physically removed from the database. List and lookup queries filter out deleted records automatically.

## Contributing

1. Create a feature branch from `main`.
2. Make your changes.
3. Ensure the project builds and (if applicable) tests pass.
4. Open a PR describing the change, including any relevant Postman collection updates.

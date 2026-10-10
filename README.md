# AprilBookStore

[![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat&logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-6.0-512BD4?style=flat&logo=nuget&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC292B?style=flat&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**AprilBookStore** is an enterprise-grade e-commerce bookstore web application built with **ASP.NET Core 6.0 MVC** following **Clean Architecture** principles. It features end-to-end book shopping and order management workflows alongside administrative operations, role- and policy-based access control, automated database seeding, and auditing.

---

## Table of Contents

- [Architecture & Design](#architecture--design)
- [Features](#features)
- [Technologies & Libraries](#technologies--libraries)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation & Setup](#installation--setup)
  - [Configuration](#configuration)
  - [Database Migrations & Seeding](#database-migrations--seeding)
  - [Running the Application](#running-the-application)
- [Default Roles & Accounts](#default-roles--accounts)
- [Contributing](#contributing)
- [License](#license)

---

## Architecture & Design

The solution is organized according to **Clean Architecture / Layered Architecture** with strict separation of concerns and dependency inversion:

```
┌────────────────────────────────────────────────────────┐
│                   AprilBookStore.Web                   │
│        (Controllers, Razor Views, ViewModels, Auth)     │
└──────────────┬──────────────────────────┬──────────────┘
               │                          │
               ▼                          ▼
┌──────────────────────────────┐  ┌──────────────────────────────┐
│  AprilBookStore.Application  │  │ AprilBookStore.Infrastructure│
│ (Services, DTOs, Interfaces) │  │  (EF Core, Interceptors,     │
└──────────────┬───────────────┘  │   Migrations, Storage)       │
               │                  └──────────────┬───────────────┘
               ▼                                 ▼
┌────────────────────────────────────────────────────────┐
│                  AprilBookStore.Domain                 │
│              (Entities, Base Entity, Enums)            │
└────────────────────────────────────────────────────────┘
```

- **Domain Layer (`AprilBookStore.Domain`)**: Contains the core domain models and business entities with zero external infrastructure dependencies.
- **Application Layer (`AprilBookStore.Application`)**: Houses use cases, business logic, DTOs, interfaces, and service implementations (`BookService`, `AuthorService`, `OrderService`, etc.).
- **Infrastructure Layer (`AprilBookStore.Infrastructure`)**: Manages external concerns such as Entity Framework Core persistence, SQL Server integration, audit interceptors, database seeding, and file storage.
- **Presentation Layer (`AprilBookStore.Web`)**: ASP.NET Core MVC application providing user interfaces, Razor Views, authentication/authorization pipelines, and controller endpoints.

---

## Features

### 🛒 Customer Experience & Storefront
- **Catalog Browsing**: Search books by title, filter by category or author, and browse with paginated results powered by `X.PagedList`.
- **Book Details**: Comprehensive book metadata including authors, categories, ISBN, pricing, and cover images.
- **Shopping Cart**: Real-time shopping cart management (add, update quantities, delete items, view subtotal).
- **Checkout & Orders**: Secure order processing and order history tracking.

### 🛡️ Administration & Operations
- **Book Management**: Full CRUD operations with support for image uploads stored on disk (`IFileStorageService`).
- **Authors & Categories**: Manage authors and genre categories with soft deletion (`IsDeleted`) and visibility toggling (`IsVisible`).
- **User & Role Administration**: Manage system users, role assignments, and custom claims directly from the admin dashboard.
- **Granular Security Policies**: Role and policy-based authorization (`SuperAdmin`, `Admin`) protecting sensitive operations (e.g. `DeleteRolePolicy`, `EditRolePolicy`, `DeleteUserPolicy`).

### ⚙️ Cross-Cutting Capabilities
- **Audit Interceptor**: Automated tracking of entity creation and modification timestamps (`CreatedDate`, `UpdatedDate`) via `AuditableEntityInterceptor`.
- **Authentication**: ASP.NET Core Identity with support for external login via Google OAuth 2.0.
- **CSRF / Antiforgery Protection**: Configured with automatic token validation (`AutoValidateAntiforgeryTokenAttribute`).
- **Automated Seeding**: Built-in seeding for initial authors, categories, books, identity roles, and optional SuperAdmin user.

---

## Technologies & Libraries

- **Framework**: [.NET 6.0](https://dotnet.microsoft.com/)
- **Web**: [ASP.NET Core MVC 6.0](https://learn.microsoft.com/aspnet/core/mvc/overview)
- **Data Access & ORM**: [Entity Framework Core 6.0](https://learn.microsoft.com/ef/core/) (SQL Server Provider)
- **Identity & Security**: ASP.NET Core Identity, Google OAuth 2.0 Authentication
- **Pagination**: [X.PagedList](https://github.com/dncuug/X.PagedList) & [X.PagedList.Mvc.Core](https://github.com/dncuug/X.PagedList)
- **Frontend & UI**: Razor Views, Bootstrap 5, jQuery, jQuery Validation & Unobtrusive Ajax

---

## Project Structure

```plaintext
BookStoreDotNetCore/
├── AprilBookStore.Domain/                # Core domain entities
│   └── Entities/
│       ├── ApplicationUser.cs           # Extended Identity user
│       ├── Author.cs                    # Author entity
│       ├── Book.cs                      # Book catalog item
│       ├── CartItem.cs                  # User shopping cart line item
│       ├── Category.cs                  # Book category/genre
│       ├── Entity.cs                    # Base entity with audit fields
│       ├── Order.cs                     # Customer order
│       └── OrderItem.cs                 # Order line item
│
├── AprilBookStore.Application/           # Business logic & contracts
│   ├── Interfaces/                      # Service and DbContext interfaces
│   ├── Models/                          # DTOs grouped by domain feature
│   ├── Security/                        # ClaimsStore and authorization models
│   └── Services/                        # Application service implementations
│
├── AprilBookStore.Infrastructure/        # Data access & external services
│   ├── Data/
│   │   ├── Interceptors/                # AuditableEntityInterceptor
│   │   ├── BookStoreContext.cs          # EF Core DbContext
│   │   ├── DbInitializer.cs             # Identity and SuperAdmin seeder
│   │   └── ModelBuilderExtensions.cs    # Catalog data seeder
│   ├── Migrations/                      # EF Core schema migrations
│   └── Services/                        # FileStorageService implementation
│
├── AprilBookStore.Web/                   # Presentation layer (ASP.NET Core MVC)
│   ├── Areas/Identity/                  # ASP.NET Core Identity Razor Pages
│   ├── Controllers/                     # MVC Controllers
│   ├── Extensions/                      # Web and application helper extensions
│   ├── ViewModels/                      # View models for Razor views
│   ├── Views/                           # Razor view templates
│   ├── wwwroot/                         # Static assets (CSS, JS, book covers)
│   ├── appsettings.json                 # Environment configuration
│   └── Program.cs                       # Application entry point & DI pipeline
│
└── AprilBookStore.sln                    # Visual Studio Solution
```

---

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) (or later with .NET 6 targeting pack)
- [Microsoft SQL Server](https://www.microsoft.com/sql-server) or SQL Server LocalDB
- [EF Core CLI Tools](https://learn.microsoft.com/ef/core/cli/dotnet) (install via `dotnet tool install --global dotnet-ef`)
- Optional: Google Cloud Console credentials for Google OAuth

### Installation & Setup

1. **Clone the repository**:
   ```bash
   git clone https://github.com/AbdurrahmanHassouna/BookStoreDotNetCore.git
   cd BookStoreDotNetCore
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

### Configuration

Update your database connection string in `AprilBookStore.Web/appsettings.json` or `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnections": "Server=(localdb)\\mssqllocaldb;Database=AprilBookStore;Trusted_Connection=True;MultipleActiveResultSets=true;"
  }
}
```

> [!NOTE]
> Ensure the connection string key matches `"DefaultConnections"`.

#### Secure Secrets Configuration (Recommended)

Use .NET **User Secrets** in development to configure sensitive credentials without committing them to source control:

```bash
cd AprilBookStore.Web

# Configure SuperAdmin bootstrap account
dotnet user-secrets set "SuperAdmin:Email" "admin@aprilbookstore.com"
dotnet user-secrets set "SuperAdmin:Password" "YourSecurePassword123!"
dotnet user-secrets set "SuperAdmin:UserName" "admin"

# (Optional) Configure Google OAuth login
dotnet user-secrets set "ClientId" "YOUR_GOOGLE_CLIENT_ID"
dotnet user-secrets set "ClientSecret" "YOUR_GOOGLE_CLIENT_SECRET"

cd ..
```

### Database Migrations & Seeding

Apply EF Core migrations to create the database schema:

```bash
dotnet ef database update --project AprilBookStore.Infrastructure --startup-project AprilBookStore.Web
```

When the application starts, it automatically seeds:
- Initial Authors, Categories, and Books.
- Default Identity Roles: `SuperAdmin` and `Admin`.
- The initial SuperAdmin user if configured via User Secrets or configuration settings.

### Running the Application

Launch the web project from the solution root:

```bash
dotnet run --project AprilBookStore.Web
```

Navigate to `https://localhost:7147` or `http://localhost:5147` (or the URL displayed in your terminal output).

---

## Default Roles & Accounts

The system implements role-based and policy-based authorization:

| Role | Access Scope |
| :--- | :--- |
| **SuperAdmin** | Full system access: manage users, delete/edit roles, manage permissions, manage catalog & orders |
| **Admin** | Catalog management: add/edit books, authors, categories, manage orders |
| **User** | Default registered customer: browse catalog, manage shopping cart, place and view orders |

---

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create your feature branch (`git checkout -b feature/AmazingFeature`).
3. Commit your changes (`git commit -m "Add AmazingFeature"`).
4. Push to the branch (`git push origin feature/AmazingFeature`).
5. Open a Pull Request.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

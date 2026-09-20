# AprilBookStore

AprilBookStore is an ASP.NET Core MVC-based web application for managing and operating an online bookstore. Users can browse books, add them to a shopping cart, and complete purchases. Admins can manage books, authors, categories, and orders. The project follows a layered architecture with separation of concerns for maintainability and scalability.

## Features

- User-friendly browsing of books.
- Shopping cart functionality.
- Secure user authentication & authorization with ASP.NET Identity.
- Admin panel for managing books, authors, and categories.
- Integration with Entity Framework Core for database operations.

## Table of Contents
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
- [Contribution](#contribution)

---

## Project Structure

```plaintext
AprilBookStore/
│
├── wwwroot/                 # Static files (CSS, JS, images)
│   ├── Assets/
│   ├── css/
│   ├── js/
│   └── lib/
│
├── Areas/
│   └── Identity/            # ASP.NET Identity for authentication
│
├── Controllers/             # Handles HTTP requests and returns views
│   ├── AdministrationController.cs
│   ├── AuthorsController.cs
│   ├── BooksController.cs
│   ├── CartController.cs
│   ├── CategoryController.cs
│   ├── HomeController.cs
│   ├── OrdersController.cs
│   └── ErrorController.cs
│
├── DataAccess/              # Database context and data services
│   ├── BookStoreContext.cs
│   ├── IData.cs
│   └── DataContext.cs
│
├── Extensions/              # Extension methods (ModelBuilder seeding)
│   └── ModelBuilderExtensions.cs
│
├── Migrations/              # Database migration files
│   ├── 20240718145305_Initial.cs
│   └── BookStoreContextModelSnapshot.cs
│
├── Models/                  # Application domain models
│
├── Security/                # Authorization handlers and requirements
│
├── ViewModels/              # ViewModels for data transfer between controllers and views
│
├── Views/                   # Razor views for rendering HTML pages
│
├── appsettings.json         # Application configuration
└── Program.cs               # Application entry point and service pipeline
```
## Configuration

### Database

This application uses SQL Server as the database provider. The connection string is defined in appsettings.json: 
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=AprilBookStore;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```
!! Make sure to replace YOUR_SERVER_NAME with the name of your local SQL Server instance or your database connection string.



## Technologies Used
- ASP.NET Core MVC - Framework for building the web application.
- Entity Framework Core - ORM for database operations.
- SQL Server - Database provider.
- Bootstrap - Front-end framework for responsive design.
## Getting Started
### Prerequisites
- .NET 6 SDK - Make sure you have .NET 6 installed.
- SQL Server - Required for database management.
### Steps to Run
- Clone the repository: 
```bash
git clone https://github.com/AbdurrahmanHassouna/Book-Store.git

```
- move to project folder
```bash
cd AprilBookStore

```
- Open the project in Visual Studio or your preferred editor.
- Update appsettings.json with your SQL Server connection string.
- Apply migrations and run the project:
```CLI
dotnet ef database update

```
- Run the project
```CLI 
dotnet run

```


## License
This project is licensed under the MIT License - see the LICENSE file for details.

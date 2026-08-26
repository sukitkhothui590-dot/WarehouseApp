# Coding Test

## Overview

This solution contains a practical warehouse management web application and a separate C# console program that intentionally demonstrates a runtime error.

## Projects

### 1. Warehouse Management System

WarehouseApp is an ASP.NET Core MVC application for managing products, receiving stock, withdrawing stock, viewing current inventory, and auditing stock movement history.

### 2. Runtime Error Demo

RuntimeErrorDemo compiles successfully, then intentionally throws System.FormatException when the user enters non-numeric input such as ABC.

## Technologies

- C# / .NET 9
- ASP.NET Core MVC and Razor Views
- Entity Framework Core 9
- SQLite
- Bootstrap and vanilla CSS/JavaScript

## Requirements

- .NET 9 SDK or later
- No external database server is required.

## How to Run

From the repository root:

    dotnet restore
    dotnet build

Run the warehouse application:

    cd WarehouseApp
    dotnet run

The application automatically applies EF Core migrations and seeds five sample products on startup. Open the URL printed by ASP.NET Core, usually http://localhost:5000 or the HTTPS URL shown in the terminal.

### Reviewer quick start

    git clone https://github.com/sukitkhothui590-dot/WarehouseApp.git
    cd WarehouseApp
    dotnet restore
    dotnet run --project WarehouseApp/WarehouseApp.csproj

The warehouse database is created automatically on first startup. No database server or manual database file is required.

Run the runtime error demo:

    cd RuntimeErrorDemo
    dotnet run

Enter 10 to see normal output. Enter ABC to intentionally produce System.FormatException.

## Database

WarehouseApp uses SQLite at WarehouseApp/warehouse.db. The database is created and migrated automatically on startup. The schema is defined by EF Core migrations in WarehouseApp/Data/Migrations; the database file is ignored by Git.

## Features

- Dashboard metrics: products, units in stock, received/withdrawn today, low stock, and recent transactions
- Product create, edit, list, and detail history
- Inventory table with code/name search and stock status
- Receive stock and withdraw stock workflows
- Transaction history with Receive/Withdraw and product filters
- Server-side and browser-side validation
- Friendly success/error feedback
- Responsive business/admin layout
- Local product images with a fallback asset; see IMAGE-SOURCES.md for source pages and usage notes.

## Project Structure

    CodingTest/
    ├── WarehouseApp/
    │   ├── Controllers/
    │   ├── Data/
    │   │   └── Migrations/
    │   ├── Models/
    │   ├── Services/
    │   ├── ViewModels/
    │   ├── Views/
    │   └── wwwroot/
    ├── RuntimeErrorDemo/
    ├── CodingTest.sln
    └── README.md

## Business Rules

- Product code is unique; name and unit are required.
- New products start with quantity 0.
- Receive and withdraw quantities must be greater than zero.
- A withdrawal cannot exceed current stock, so stock can never become negative.
- Each stock change records BalanceBefore, BalanceAfter, type, quantity, note, and timestamp.
- Product quantity update and transaction insert run in one database transaction. A failure rolls both back.
- Product deletion is intentionally omitted so transaction history cannot be orphaned.

## Runtime Error Explanation

Compilation succeeds because the compiler can validate syntax, types, and that int.Parse(string) is a valid method call. It cannot know what text a user will type at runtime.

When the user enters ABC, the program executes int.Parse("ABC"). Since that text is not an integer, .NET throws System.FormatException.

A safe version would be:

    if (int.TryParse(input, out int quantity))
    {
        Console.WriteLine(quantity);
    }
    else
    {
        Console.WriteLine("Invalid quantity");
    }

That fix is deliberately not used in the main RuntimeErrorDemo, because this task requires the intentional runtime failure.

## Testing

Build both projects:

    dotnet build CodingTest.sln

Warehouse runtime checks:

1. Create P100 / Test Product / pcs; verify stock is 0.
2. Receive 10; verify an IN transaction with before 0, after 10.
3. Withdraw 4; verify an OUT transaction with before 10, after 6.
4. Withdraw 10; verify rejection, stock remains 6, and no invalid transaction is added.
5. Submit 0 or -1; verify validation rejection.
6. Create P100 again; verify duplicate code rejection.
7. Check inventory, product detail, dashboard, and transaction history.

Runtime demo check:

    dotnet build RuntimeErrorDemo
    dotnet run --project RuntimeErrorDemo
    # Enter: ABC
    # Expected: System.FormatException

## Design Decisions

- ASP.NET Core MVC and Razor keep the coding-test focus on C#, database access, and business logic without an unnecessary SPA.
- SQLite makes cloning and reviewing the project easy without installing a database server.
- EF Core migrations provide a repeatable schema and seed process.
- InventoryService centralizes stock rules so controllers remain thin.
- Transaction history is a first-class entity and is written atomically with the product quantity update.

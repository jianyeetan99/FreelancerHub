# FreelancerHub

A clean architecture .NET 8 project to manage a freelancer directory with secure API and Razor Pages UI.

---

## ✨ Features

- **ASP.NET Core Web API** with RESTful endpoints
- **Clean Architecture** separation (API, Application, Domain, Infrastructure)
- **JWT Authentication** (access & refresh tokens)
- **Razor Pages UI**
  - Login / Logout
  - List freelancers
  - Add / Edit / Delete freelancers
  - Wildcard search by username/email
  - Archive / Unarchive freelancers
- **Dapper** for fast, lightweight SQL Server access
- **FluentValidation** for request validation
- **MediatR** for CQRS and decoupled handlers
- **Bootstrap** styling and icons
- **Pagination-ready** API

---

## 🏗️ Project Structure

```

FreelancerHub
├── API               # ASP.NET Core API
│   └── Controllers
├── Application       # Application layer
│   ├── DTOs
│   ├── Handlers
│   └── Interfaces
├── Domain            # Domain entities
├── Infrastructure    # Dapper and repository implementations
├── UI                # Razor Pages frontend
└── Shared            # Common settings and constants

````

---

## 🔑 Getting Started

### 1️⃣ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server or Azure SQL
- Visual Studio or JetBrains Rider

---

### 2️⃣ Database Setup

1. Create your database (e.g., `FreelancerHubDb`)
2. Create the tables:

```sql
CREATE TABLE Freelancers (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Username NVARCHAR(100),
    Email NVARCHAR(100),
    PhoneNumber NVARCHAR(20),
    IsArchived BIT
);

CREATE TABLE FreelancerSkills (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FreelancerId UNIQUEIDENTIFIER,
    Skill NVARCHAR(100)
);

CREATE TABLE FreelancerHobbies (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FreelancerId UNIQUEIDENTIFIER,
    Hobby NVARCHAR(100)
);

CREATE TABLE RefreshTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId NVARCHAR(100) NOT NULL,
    Token NVARCHAR(200) NOT NULL,
    ExpiryDate DATETIME NOT NULL
);
````

3. Update `ConnectionStrings` in `appsettings.json`.

---

### 3️⃣ Running the API

In `FreelancerHub.Api`:

```bash
dotnet run
```

Swagger UI will be available at:

```
https://localhost:{port}/swagger
```

---

### 4️⃣ Running the Razor Pages UI

In `FreelancerHub.UI`:

```bash
dotnet run
```

The UI will prompt for login and allow full CRUD.

---

## 🔒 Authentication Flow

* **Login** returns an access token (JWT) and refresh token
* **TokenStorage** stores the JWT in cookies
* Each API call uses Bearer authentication

---

## 💡 Design Patterns Used

* CQRS with MediatR
* Clean Architecture
* FluentValidation for input validation
* Repository pattern with Dapper
* Dependency Injection everywhere


## 📝 License

This project is for assessment and learning purposes.


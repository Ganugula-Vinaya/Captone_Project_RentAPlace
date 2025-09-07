# RentAPlace - Backend (API)

A clean, step-by-step README for the backend API included in the `RentAPlace.Api` folder. This guide explains how to configure, build, run, and test the backend on your local machine.

---

## Table of contents
1. Project overview
2. Prerequisites
3. Quick start (run locally)
4. Configuration (appsettings)
5. Database (EF Core migrations)
6. Running the API
7. API surface (important endpoints)
8. Email & images
9. Running tests
10. Troubleshooting & tips
11. Useful commands

---

## 1) Project overview
This repository contains the backend API project `RentAPlace.Api` (ASP.NET Core Web API, **.NET 8**). It provides endpoints for authentication, property listings, reservations, messaging, and reviews. Swagger is enabled in Development environment.

Key packages (see `RentAPlace.Api.csproj`):
- Microsoft.EntityFrameworkCore (SQL Server)
- Microsoft.AspNetCore.Authentication.JwtBearer
- Swashbuckle.AspNetCore
- BCrypt.Net-Next (password hashing)

There is a small set of sample images under `wwwroot/uploads/properties/` used by the API.

---

## 2) Prerequisites
- .NET SDK 8.x (installed and on your PATH). Check with:

```bash
dotnet --version
```

- SQL Server or SQL Server Express / LocalDB (or any SQL Server-compatible instance)
- (Optional) `dotnet-ef` tool for migrations:

```bash
dotnet tool install --global dotnet-ef
```

- Git (optional) and a terminal (PowerShell, bash, etc.)

---

## 3) Quick start (run locally)
1. Unzip your uploaded archive and open a terminal in the `RentAPlace.Api` folder.

```powershell
cd path/to/RentAPlace.Api
```

2. Restore dependencies and build:

```bash
dotnet restore
dotnet build
```

3. Update configuration (see next section) before first run.

4. Run the API:

```bash
dotnet run
```

By default Kestrel will print the listening URL(s) (e.g. `https://localhost:5264`). Open `https://localhost:5264/swagger` while `ASPNETCORE_ENVIRONMENT=Development` to view the API docs.

---

## 4) Configuration (`appsettings.json`)
The project comes with `appsettings.json` and `appsettings.Development.json`. Important configuration keys:

- `ConnectionStrings:DefaultConnection` — SQL Server connection string (update for your machine). Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=LAPTOP-MIKLI6T8\\SQLEXPRESS;Database=RentAPlaceDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

- `Jwt` — JSON Web Token settings (**important**: key must be at least **32 characters** for HS256):

```json
"Jwt": {
  "Key": "<Your-very-long-secret-key-32+chars>",
  "Issuer": "RentAPlace",
  "Audience": "RentAPlaceAudience",
  "ExpiryMinutes": 60
}
```

- `Email` — SMTP settings (used for sending email):

```json
"Email": {
  "SmtpHost": "smtp.example.com",
  "SmtpPort": 587,
  "User": "smtp-user",
  "Password": "smtp-password",
  "From": "noreply@rentaplace.local",
  "UseSsl": true
}
```

> Tip: For development you can use Mailtrap or similar test SMTP providers. Keep secrets out of source control — use environment variables or user secrets for production.

To run in development mode (Swagger enabled):

Command Prompt:
```cmd:
dotnet run
```

Linux/macOS:
```bash
export ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

---

## 5) Database (EF Core migrations)
The project uses EF Core with `AppDbContext`. There may not be pre-generated migrations in this package — create them locally and apply.

1. Install EF Core tools if you haven't:

```bash
dotnet tool install --global dotnet-ef
```

2. Create the first migration (from the `RentAPlace.Api` project folder):

```bash
dotnet ef migrations add InitialCreate
```

3. Apply the migration to your database:

```bash
dotnet ef database update
```

If you prefer, run the API and implement a small script to call `context.Database.EnsureCreated()` (not recommended for production migrations).

---

## 6) Running the API
From the `RentAPlace.Api` directory:

```bash
dotnet run
# or for auto-reload during development
dotnet watch run
```

Open Swagger (Development): `https://localhost:<port>/swagger` to explore endpoints and test them.

When calling secured endpoints you must pass JWT in the `Authorization` header:

```
Authorization: Bearer <token>
```

To get a token: `POST /api/auth/login` (see DTOs under `RentAPlace.Api/Dtos`).

---

## 7) API surface (important endpoints)
Below is a condensed list of controllers & routes (found in `Controllers/`):

- **AuthController** (`/api/auth`)
  - `POST /api/auth/register` — register new user
  - `POST /api/auth/login` — authenticate & receive JWT

- **PropertiesController** (`/api/properties`)
  - `GET /api/properties/search` — search/list properties
  - `GET /api/properties/{id}` — get details
  - `POST /api/properties` — (owner) create property
  - `POST /api/properties/{id}/images` — upload property images
  - `PUT /api/properties/{id}` — update property
  - `DELETE /api/properties/{id}` — delete property
  - `GET /api/properties/mine` — properties for current owner
  - `GET /api/properties/amenities` — list available amenities

- **ReservationsController** (`/api/reservations`)
  - `POST /api/reservations` — create reservation
  - `POST /api/reservations/{id}/confirm` — owner confirms
  - `POST /api/reservations/{id}/cancel` — cancel
  - `GET /api/reservations/owner` — reservations for owner
  - `GET /api/reservations/mine` — reservations for current renter

- **MessagesController** (`/api/messages`)
  - `POST /api/messages` — contact owner/send message
  - `GET /api/messages/property/{propertyId}` — messages for property
  - `PATCH /api/messages/{id}/read` — mark as read

- **ReviewsController** (`/api/reviews`)
  - `GET /api/reviews/property/{propertyId}` — get reviews for property
  - `POST /api/reviews` — create review

> For full request/response shapes, see DTOs in `RentAPlace.Api/Dtos` (e.g. `AuthDtos.cs`, `PropertyDtos.cs`, `ReservationDtos.cs`).

---

## 8) Email & images
- **Images** are stored in `wwwroot/uploads/properties/` and served as static files. Sample images are included in the project.
- **Email**: SMTP config is read from `appsettings.json`. For development you can use Mailtrap credentials (already present in the sample config). Replace with real SMTP credentials in production.

---

## 9) Running tests
If you have a test project (`RentAPlace.Tests`) at solution root, run all tests with:

```bash
# from solution root (or in the tests folder)
dotnet test
```

If you encounter warnings about missing referenced projects (for example `RentAPlace.Domain`), make sure all projects in the solution are present or remove broken references from the `.csproj` files used only for local testing.

---

## 10) Troubleshooting & tips
- **JWT key size error** (`IDX10720` complaining key size): ensure `Jwt:Key` has **at least 32 characters** (256 bits) for HS256. Example:

```json
"Jwt:Key": "ThisIsA_VeryLong_Jwt_Secret_Key_32+Chars"
```

- **Swagger not visible**: set `ASPNETCORE_ENVIRONMENT=Development` before running.

- **EF migration errors**: install/upgrade `dotnet-ef` and ensure `Microsoft.EntityFrameworkCore.Tools` is referenced in the project.

- **SMTP issues**: For local testing use Mailtrap. For production use TLS-enabled SMTP credentials and set `Email:UseSsl` appropriately.

- **Tests referencing missing projects**: open the test project's `.csproj` and remove or fix any `<ProjectReference>` entries that point to non-existent projects.

---

## 11) Useful commands (summary)
```bash
# Restore/build
dotnet restore
dotnet build

# Run
dotnet run
# OR
dotnet watch run

# Run migrations (if needed)
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update

# Tests (if you have a tests project)
dotnet test
```






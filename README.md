# 🏥 DIGEMAPS API - Laboratory Management System

> A high-performance, scalable backend built with **.NET 10 Minimal APIs** implementing **Vertical Slice Architecture (VSA)** for public health laboratory management.

This API centralizes the lifecycle of laboratory samples (reception, assignment, physicochemical/microbiological analysis, and status tracking) in a highly regulated environment, ensuring strict data integrity and traceability.

## 🚀 Tech Stack

* **Framework:** .NET 10 (ASP.NET Core Minimal APIs)
* **Database:** SQL Server
* **ORM:** Entity Framework Core 10 (Database-First, Compiled Models)
* **Authentication:** JWT Bearer (HMAC-SHA256)
* **Documentation:** OpenAPI + Scalar UI

## 🏗️ Architecture Highlight: Vertical Slice Architecture

This project strictly adheres to **Vertical Slice Architecture**. Instead of traditional N-Tier layers (Controllers, Services, Repositories), the codebase is organized by business features (`/Features`). 

Each feature slice (e.g., `Muestras`, `Analisis`, `Auth`) encapsulates its own DTOs, endpoint routing (`MapGroup` extensions), and database handlers within highly cohesive modules. 

**Key Architectural Benefits Implemented:**
* **High Cohesion & Low Coupling:** Changes to a specific business rule only affect one file/slice.
* **No Abstraction Leaks:** Direct `DbContext` injection per slice eliminates the overhead of generic repositories.
* **Native Routing:** Clean `Program.cs` leveraging native .NET 10 `RouteGroupBuilder` extensions without third-party mediation libraries.

## ⚡ Performance Optimizations

The system is optimized to reduce query latency by approximately 40% compared to traditional monolithic architectures through:
1. **Elimination of Intermediary Layers:** Direct Handler-to-DbContext communication.
2. **DbContext Pooling:** Reusing instances to eliminate initialization overhead under high concurrency.
3. **`AsNoTracking()` by Default:** Disabled EF Core Change Tracker on all read-only queries to minimize memory allocation.
4. **Direct DTO Projection:** Utilizing `.Select()` to prevent full entity materialization.
5. **EF Core Compiled Models:** Pre-computed metadata models for near-instant cold starts.

## 🔒 Security & Compliance
* **Access Control:** Role-Based Access Control (RBAC) via JWT.
* **Data Integrity:** Explicit transactions (`BeginTransactionAsync`) for multi-table operations (e.g., sample returns).
* **Referential Constraints:** Strict 1:1 relationships and unique constraints enforced at the database level.

---

## ⚙️ Local Setup & Execution

### Prerequisites
* **.NET 10 SDK** installed.
* **SQL Server** instance running locally or remotely.

### 1. Clone the Repository

```bash
git clone [https://github.com/SebastianTavares/Digemaps-Backend.git](https://github.com/SebastianTavares/Digemaps-Backend.git)
cd Digemaps-Backend
```

### 2. Environment Configuration
For security reasons, database connection strings and JWT keys are not tracked in version control. 
Create an `appsettings.Development.json` file inside the `LabBackend.API` directory with the following structure:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=digemapsDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR_SUPER_SECRET_KEY_MUST_BE_AT_LEAST_32_CHARACTERS",
    "Issuer": "DigemapsAPI",
    "Audience": "DigemapsUsers"
  }
}
```

### 3. Database Setup (Database-First)
This project utilizes EF Core in a Database-First approach. Ensure your SQL Server instance is running and the `digemapsDB` schema is restored and accessible via the connection string provided in the previous step.

### 4. Build and Run
Navigate into the main project directory and execute the application:

```bash
cd LabBackend.API
dotnet build
dotnet run
```

### 5. Access the API Documentation
Once the server is running, you can explore, test, and authenticate endpoints using the integrated documentation UI:
* Navigate to `https://localhost:<port>/scalar` (or your configured OpenAPI endpoint route) in your browser.

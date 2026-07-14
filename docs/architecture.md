# MoneyFlow Architecture

## Overview

MoneyFlow will use a layered architecture inspired by Clean Architecture.

The goal is to keep business logic independent from frameworks, databases and external services, while avoiding unnecessary complexity.

## Solution Structure

```txt
MoneyFlow.sln
│
├── MoneyFlow.Domain
├── MoneyFlow.Application
├── MoneyFlow.Infrastructure
└── MoneyFlow.API
```

## Project Responsibilities
MoneyFlow.Domain

Contains the core business model and business rules.

It includes:
- Entities
- Enums
- Value objects, only when they provide real value
- Domain exceptions
- Repository contracts required by the business

The Domain project must not depend on the other projects.

### Examples:
- User
- Category
- Transaction
- SavingsGoal
- MonthlyPlan
- FinancialInsight


## MoneyFlow Application

Contains the use cases of the system.

It coordinates the domain to perform actions requested by the user.

It includes:

- Application services
- Commands and queries when they improve clarity
- Request and response models
- Validation
- Interfaces for external services

### Examples:
- RegisterUser
- LoginUser
- CreateTransaction
- GetMonthlySummary
- CreateSavingsGoal

## MoneyFlow.Infrastructure

Contains technical implementations.

It includes:

- Entity Framework Core
- MySQL configuration
- Repository implementations
- Password hashing
- JWT generation
- Database migrations
- External service implementations

The Infrastructure project depends on Domain and Application.

## MoneyFlow.API

Exposes the application through HTTP.

It includes:

- Controllers
- Authentication configuration
- Dependency injection
- Middleware
- Swagger / OpenAPI
- Error handling
- Application startup configuration

The API project depends on Application and Infrastructure.

## Dependency Direction

```txt
API ───────────────┐
                   ▼
             Application
                   ▼
                Domain

Infrastructure ───► Application
Infrastructure ───► Domain
```

The Domain layer does not depend on frameworks or infrastructure.

## Request Flow
Example: creating an expense.
```txt
HTTP Request
    ↓
TransactionsController
    ↓
Application use case
    ↓
Domain entity and business rules
    ↓
Repository contract
    ↓
Infrastructure repository
    ↓
MySQL database
```

## Architectural Principles
- Business rules belong in Domain.
- Use cases belong in Application.
- Database code belongs in Infrastructure.
- HTTP concerns belong in API.
- Controllers should remain small.
- Entities should protect their own valid state.
- Abstractions will be introduced only when they solve a real problem.
- MoneyFlow will not use patterns only for the sake of appearing complex.

## Initial Technical Stack
### Backend
- ASP.NET Core
- Entity Framework Core
- MySQL
- JWT authentication
- Swagger / OpenAPI
- FluentValidation when validation becomes complex
- xUnit for automated tests
- Docker for local and deployment environments

### Frontend
- Angular
- Angular Material
- RxJS
- Angular Signals
- Chart.js
- SCSS

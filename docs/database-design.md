# MoneyFlow Database Design

## Overview

MoneyFlow will use a relational database with MySQL.

The database is designed around the main financial concepts of the application:

- Users
- Categories
- Transactions
- Savings goals
- Monthly plans
- Category limits
- Financial insights

---

## Tables

### Users

Stores registered MoneyFlow users.

Fields:

- Id
- FirstName
- LastName
- Email
- PasswordHash
- CreatedAt

Rules:

- Email must be unique.
- FirstName is required.
- LastName is required.
- PasswordHash is required.

---

### Categories

Stores categories used to classify transactions.

Fields:

- Id
- UserId
- Name
- Type
- Icon

Rules:

- A category belongs to one user.
- Name is required.
- Type can be Income or Expense.
- A user should not have duplicate category names for the same type.

---

### Transactions

Stores income and expense movements.

Fields:

- Id
- UserId
- CategoryId
- Amount
- Description
- Date
- Type

Rules:

- A transaction belongs to one user.
- A transaction belongs to one category.
- Amount must be greater than zero.
- Type can be Income or Expense.
- The transaction type must match the category type.

---

### SavingsGoals

Stores financial goals created by users.

Fields:

- Id
- UserId
- Title
- TargetAmount
- CurrentAmount
- Deadline

Rules:

- A savings goal belongs to one user.
- TargetAmount must be greater than zero.
- CurrentAmount cannot be negative.
- CurrentAmount should not exceed TargetAmount.
- Deadline is optional.

---

### MonthlyPlans

Stores the financial plan for a specific user and month.

Fields:

- Id
- UserId
- Month
- Year
- ExpectedIncome
- TargetSavings
- TotalSpendingLimit
- Currency
- CreatedAt

Rules:

- A monthly plan belongs to one user.
- A user can have only one plan per month and year.
- ExpectedIncome cannot be negative.
- TargetSavings cannot be negative.
- TotalSpendingLimit cannot be negative.
- Month must be between 1 and 12.
- Currency is required.

---

### MonthlyPlanCategoryLimits

Stores the spending limit assigned to each category within a monthly plan.

Fields:

- Id
- MonthlyPlanId
- CategoryId
- LimitAmount

Rules:

- A category limit belongs to one monthly plan.
- A category limit belongs to one category.
- LimitAmount must be greater than or equal to zero.
- A category can appear only once inside the same monthly plan.

---

### FinancialInsights

Stores recommendations or alerts generated from financial behavior.

Fields:

- Id
- UserId
- Title
- Description
- Type
- Severity
- CreatedAt

Rules:

- A financial insight belongs to one user.
- Title is required.
- Description is required.
- Severity can be Informational, Warning or Critical.

---

## Relationships

```txt
User 1 ─────── N Category

User 1 ─────── N Transaction

User 1 ─────── N SavingsGoal

User 1 ─────── N MonthlyPlan

User 1 ─────── N FinancialInsight

Category 1 ─── N Transaction

MonthlyPlan 1 ─── N MonthlyPlanCategoryLimit

Category 1 ────── N MonthlyPlanCategoryLimit

```
## Important Constraints

### Unique user email
`Users.Email` must be unique.

### Unique monthly plan
`UserId + Month + Year` must be unique.

### Unique category inside a monthly plan
`MonthlyPlanId + CategoryId` must be unique.

### Unique category name by user and type
`UserId + Name + Type` must be unique.

## Deletion Behavior
For V1:
- Deleting a user deletes their personal financial data.
- A category cannot be deleted if it is already used by transactions.
- Deleting a monthly plan deletes its category limits.
- Deleting a savings goal does not affect transactions.
- Deleting a financial insight does not affect financial records.
# MoneyFlow Enums and Types

## Enums

### TransactionType

Represents the type of a transaction.

```csharp
public enum TransactionType
{
    Income = 1,
    Expense = 2
}
```

---

### CategoryType

Represents the type of a category.

```csharp
public enum CategoryType
{
    Income = 1,
    Expense = 2
}
```

---

### FinancialInsightSeverity

Represents the importance level of a recommendation.

```csharp
public enum FinancialInsightSeverity
{
    Informational = 1,
    Warning = 2,
    Critical = 3
}
```

---

### FinancialInsightType

Represents the type of recommendation.

```csharp
public enum FinancialInsightType
{
    SpendingAlert = 1,
    SavingsProgress = 2,
    MonthlySummary = 3
}
```

---

## Data Types

### User

| Field | Type |
|--------|--------|
| Id | long |
| FirstName | string |
| LastName | string |
| Email | string |
| PasswordHash | string |
| CreatedAt | DateTime |

---

### Category

| Field | Type |
|--------|--------|
| Id | long |
| UserId | long |
| Name | string |
| Type | CategoryType |
| Icon | string |

---

### Transaction

| Field | Type |
|--------|--------|
| Id | long |
| UserId | long |
| CategoryId | long |
| Amount | decimal |
| Description | string |
| Date | DateTime |
| Type | TransactionType |

---

### SavingsGoal

| Field | Type |
|--------|--------|
| Id | long |
| UserId | long |
| Title | string |
| TargetAmount | decimal |
| CurrentAmount | decimal |
| Deadline | DateTime? |

---

### MonthlyPlan

| Field | Type |
|--------|--------|
| Id | long |
| UserId | long |
| Month | int |
| Year | int |
| ExpectedIncome | decimal |
| TargetSavings | decimal |
| TotalSpendingLimit | decimal |
| Currency | string |
| CreatedAt | DateTime |

---

### MonthlyPlanCategoryLimit

| Field | Type |
|--------|--------|
| Id | long |
| MonthlyPlanId | long |
| CategoryId | long |
| LimitAmount | decimal |

---

### FinancialInsight

| Field | Type |
|--------|--------|
| Id | long |
| UserId | long |
| Title | string |
| Description | string |
| Type | FinancialInsightType |
| Severity | FinancialInsightSeverity |
| CreatedAt | DateTime |
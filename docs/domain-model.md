# MoneyFlow Domain Model

## V1 Entities

### User
Represents a person who uses MoneyFlow to manage personal finances.

Attributes:
- Id
- FirstName
- LastName
- Email
- PasswordHash
- CreatedAt

### Category
Represents a group used to classify transactions.

Examples:
- Food
- Transport
- Education
- Entertainment
- Health
- Other

Attributes:
- Id
- Name
- Type
- Icon
- UserId

### Transaction
Represents a financial movement made by the user.

A transaction can be:
- Income
- Expense

Attributes:
- Id
- UserId
- CategoryId
- Amount
- Description
- Date
- Type

### SavingsGoal
Represents a personal financial objective.

Example:
- Buy a laptop
- Emergency fund
- Travel

Attributes:
- Id
- UserId
- Title
- TargetAmount
- CurrentAmount
- Deadline

### MonthlyPlan
Represents the financial plan created by the user for a specific month.

It defines:
- Expected monthly income
- Target savings
- Spending limits by category

Attributes:
- Id
- UserId
- Month
- Year
- ExpectedIncome
- TargetSavings
- TotalSpendingLimit
- Currency
- CreatedAt

### MonthlyPlanCategoryLimit
Represents the spending limit assigned to a category inside a monthly plan.

Attributes:
- Id
- MonthlyPlanId
- CategoryId
- LimitAmount

### FinancialInsight
Represents a message or recommendation generated from the user's financial behavior.

Example:
- You spent more than planned on food.
- You are on track to reach your savings goal.

Attributes:
- Id
- UserId
- Title
- Description
- Type
- Severity
- CreatedAt
# MoneyFlow Domain Model

## V1 Entities

### User
Represents a person who uses MoneyFlow to manage personal finances.

### Category
Represents a group used to classify transactions.

Examples:
- Food
- Transport
- Education
- Entertainment
- Health
- Other

### Transaction
Represents a financial movement made by the user.

A transaction can be:
- Income
- Expense

### SavingsGoal
Represents a personal financial objective.

Example:
- Buy a laptop
- Emergency fund
- Travel

### MonthlyPlan
Represents the financial plan created by the user for a specific month.

It defines:
- Expected monthly income
- Target savings
- Spending limits by category

### FinancialInsight
Represents a message or recommendation generated from the user's financial behavior.

Example:
- You spent more than planned on food.
- You are on track to reach your savings goal.
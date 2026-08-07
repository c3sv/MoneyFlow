using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.Accounts;

public sealed class Account
{
    private Account()
    {
    }

    public long Id { get; private set; }

    public long UserId { get; private set; }

    public string Bank { get; private set; } = null!;

    public string Nickname { get; private set; } = null!;

    public AccountType Type { get; private set; }

    public string Last4 { get; private set; } = null!;

    public decimal Balance { get; private set; }

    public string Currency { get; private set; } = null!;

    public DateTime UpdatedAt { get; private set; }

    public Account(
        long userId,
        string bank,
        string nickname,
        AccountType type,
        string last4,
        decimal initialBalance,
        string currency)
    {
        if (userId <= 0)
        {
            throw new DomainException("User id must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(bank))
        {
            throw new DomainException("Bank name is required.");
        }

        if (string.IsNullOrWhiteSpace(nickname))
        {
            throw new DomainException("Account nickname is required.");
        }

        if (string.IsNullOrWhiteSpace(last4) || last4.Trim().Length != 4)
        {
            throw new DomainException(
                "Last4 must contain exactly 4 characters.");
        }

        if (type != AccountType.Credit && initialBalance < 0)
        {
            throw new DomainException(
                "Only credit accounts can have a negative balance.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException("Currency is required.");
        }

        UserId = userId;
        Bank = bank.Trim();
        Nickname = nickname.Trim();
        Type = type;
        Last4 = last4.Trim();
        Balance = initialBalance;
        Currency = currency.Trim().ToUpperInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBalance(decimal newBalance)
    {
        if (Type != AccountType.Credit && newBalance < 0)
        {
            throw new DomainException(
                "Only credit accounts can have a negative balance.");
        }

        Balance = newBalance;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApplyTransaction(
        TransactionType transactionType,
        decimal amount)
    {
        if (amount <= 0)
        {
            throw new DomainException(
                "Transaction amount must be greater than zero.");
        }

        if (!Enum.IsDefined(transactionType))
        {
            throw new DomainException(
                "Transaction type is invalid.");
        }

        var newBalance = transactionType switch
        {
            TransactionType.Income => Balance + amount,
            TransactionType.Expense => Balance - amount,
            _ => throw new DomainException(
                "Transaction type is invalid.")
        };

        if (Type != AccountType.Credit && newBalance < 0)
        {
            throw new DomainException(
                "The account does not have enough balance.");
        }

        Balance = newBalance;
        UpdatedAt = DateTime.UtcNow;
    }
    
}
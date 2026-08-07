using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.Accounts;

public sealed class Account
{
    private const int MaxBankLength = 100;
    private const int MaxNicknameLength = 150;
    private const string SupportedCurrency = "PEN";

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
            throw new DomainException(
                "User id must be greater than zero.");
        }

        UserId = userId;
        Balance = initialBalance;
        Currency = NormalizeCurrency(currency);

        UpdateDetails(
            bank,
            nickname,
            type,
            last4);
    }

    public void UpdateDetails(
        string bank,
        string nickname,
        AccountType type,
        string last4)
    {
        if (!Enum.IsDefined(type))
        {
            throw new DomainException(
                "Account type is invalid.");
        }

        if (type != AccountType.Credit && Balance < 0)
        {
            throw new DomainException(
                "An account with a negative balance must remain a credit account.");
        }

        Bank = NormalizeBank(bank);
        Nickname = NormalizeNickname(nickname);
        Type = type;
        Last4 = NormalizeLast4(last4);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApplyTransaction(
        TransactionType transactionType,
        decimal amount)
    {
        ValidateTransaction(transactionType, amount);

        ChangeBalance(
            GetBalanceEffect(transactionType, amount));
    }

    public void ReverseTransaction(
        TransactionType transactionType,
        decimal amount)
    {
        ValidateTransaction(transactionType, amount);

        ChangeBalance(
            -GetBalanceEffect(transactionType, amount));
    }

    public void ReplaceTransaction(
        TransactionType previousType,
        decimal previousAmount,
        TransactionType newType,
        decimal newAmount)
    {
        ValidateTransaction(
            previousType,
            previousAmount);

        ValidateTransaction(
            newType,
            newAmount);

        var previousEffect =
            GetBalanceEffect(previousType, previousAmount);

        var newEffect =
            GetBalanceEffect(newType, newAmount);

        ChangeBalance(newEffect - previousEffect);
    }

    private void ChangeBalance(decimal balanceChange)
    {
        if (balanceChange == 0)
        {
            return;
        }

        var newBalance = Balance + balanceChange;

        if (Type != AccountType.Credit && newBalance < 0)
        {
            throw new DomainException(
                "The account does not have enough balance.");
        }

        Balance = newBalance;
        UpdatedAt = DateTime.UtcNow;
    }

    private static decimal GetBalanceEffect(
        TransactionType transactionType,
        decimal amount)
    {
        return transactionType switch
        {
            TransactionType.Income => amount,
            TransactionType.Expense => -amount,
            _ => throw new DomainException(
                "Transaction type is invalid.")
        };
    }

    private static void ValidateTransaction(
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
    }

    private static string NormalizeBank(string bank)
    {
        if (string.IsNullOrWhiteSpace(bank))
        {
            throw new DomainException(
                "Bank name is required.");
        }

        var normalizedBank = bank.Trim();

        if (normalizedBank.Length > MaxBankLength)
        {
            throw new DomainException(
                $"Bank name cannot exceed {MaxBankLength} characters.");
        }

        return normalizedBank;
    }

    private static string NormalizeNickname(string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
        {
            throw new DomainException(
                "Account nickname is required.");
        }

        var normalizedNickname = nickname.Trim();

        if (normalizedNickname.Length > MaxNicknameLength)
        {
            throw new DomainException(
                $"Account nickname cannot exceed {MaxNicknameLength} characters.");
        }

        return normalizedNickname;
    }

    private static string NormalizeLast4(string last4)
    {
        if (string.IsNullOrWhiteSpace(last4))
        {
            throw new DomainException(
                "Last4 is required.");
        }

        var normalizedLast4 = last4.Trim();

        if (normalizedLast4.Length != 4 ||
            normalizedLast4.Any(character =>
                !char.IsDigit(character)))
        {
            throw new DomainException(
                "Last4 must contain exactly 4 digits.");
        }

        return normalizedLast4;
    }

    private static string NormalizeCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException(
                "Currency is required.");
        }

        var normalizedCurrency =
            currency.Trim().ToUpperInvariant();

        if (normalizedCurrency != SupportedCurrency)
        {
            throw new DomainException(
                $"Only {SupportedCurrency} currency is supported.");
        }

        return normalizedCurrency;
    }
}
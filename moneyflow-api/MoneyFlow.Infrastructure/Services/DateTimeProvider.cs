using MoneyFlow.Application.Abstractions.Services;

namespace MoneyFlow.Infrastructure.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
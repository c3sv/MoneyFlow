namespace MoneyFlow.Application.Abstractions.Services;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
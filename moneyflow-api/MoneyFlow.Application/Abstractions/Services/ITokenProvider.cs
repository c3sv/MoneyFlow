using MoneyFlow.Domain.Users;

namespace MoneyFlow.Application.Abstractions.Services;

public interface ITokenProvider
{
    string Generate(User user);
}
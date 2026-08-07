using MoneyFlow.Domain.Users;

namespace MoneyFlow.Application.Abstractions.Services;

public interface ITokenProvider
{
    GeneratedAccessToken Generate(User user);
}
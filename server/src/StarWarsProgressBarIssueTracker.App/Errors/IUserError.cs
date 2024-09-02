using HotChocolate;
using ErrorCodes = StarWarsProgressBarIssueTracker.Domain.Errors.ErrorCodes;

namespace StarWarsProgressBarIssueTracker.App.Errors;

[GraphQLName("UserError")]
public interface IUserError : IError
{
    ErrorCodes UserErrorCode { get; }
}

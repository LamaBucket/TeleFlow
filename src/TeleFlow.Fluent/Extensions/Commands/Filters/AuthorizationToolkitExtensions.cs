using TeleFlow.Core.Commands.Filters;
using TeleFlow.Fluent.Builders.Commands;

namespace TeleFlow.Fluent.Extensions.Commands.Filters;

public static class AuthorizationToolkitExtensions
{
    public static CommandFilterBuilder RequireAuthorization(this CommandFilterBuilder builder, bool allow, string? forbidMessage = null, AuthorizationFilterCommandResult? commandResultFactory = null)
        => builder.RequireAuthorization(async _ => allow, forbidMessage, commandResultFactory);

    public static CommandFilterBuilder RequireAuthorization(this CommandFilterBuilder builder, Task<bool> authorizationProvider, string? forbidMessage = null, AuthorizationFilterCommandResult? commandResultFactory = null)
        => builder.RequireAuthorization((sp) => authorizationProvider, forbidMessage, commandResultFactory);

    public static CommandFilterBuilder RequireAuthorization(this CommandFilterBuilder builder, AuthorizationProvider authorizationProvider, string? forbidMessage = null, AuthorizationFilterCommandResult? commandResultFactory = null)
        => builder.AddFilter(() => new AuthorizationFilter(authorizationProvider, commandResultFactory, forbidMessage));
}

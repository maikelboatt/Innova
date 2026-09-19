namespace Application.Abstractions.Messaging
{
    /// <summary>
    ///     Marker for the small set of commands that must be reachable
    ///     without a signed-in user — currently only AuthenticateUserCommand.
    ///     AuthorizationCommandHandler checks for this before requiring
    ///     IsAuthenticated, specifically to avoid the unsolvable loop
    ///     where logging in would itself require already being logged in.
    /// </summary>
    public interface IAllowAnonymousCommand;
}

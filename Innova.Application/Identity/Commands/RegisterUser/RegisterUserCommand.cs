using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Identity.Commands.RegisterUser
{
    public sealed record RegisterUserCommand( string Username, string Password, string Role ):ICommand<Guid>;
}

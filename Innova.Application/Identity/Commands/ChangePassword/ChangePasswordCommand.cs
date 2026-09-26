using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Identity.Commands.ChangePassword
{
    public sealed record ChangePasswordCommand( string OldPassword, string NewPassword ):ICommand;
}

using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.GuestManagement.Guest.Exceptions
{
    public sealed class GuestNotFoundException( GuestId guestId ):ApplicationException($"Guest with ID '{guestId}' was not found.");
}

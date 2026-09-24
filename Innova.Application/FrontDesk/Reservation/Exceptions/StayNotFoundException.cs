using Innova.Domain.FrontDesk.ValueObjects;

namespace Innova.Application.FrontDesk.Reservation.Exceptions
{
    public sealed class StayNotFoundException( StayId stayId ):ApplicationException($"Stay '{stayId}' was not found.");
}

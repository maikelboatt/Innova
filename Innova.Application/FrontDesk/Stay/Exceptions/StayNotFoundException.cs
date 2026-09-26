using Innova.Domain.FrontDesk.ValueObjects;

namespace Innova.Application.FrontDesk.Stay.Exceptions
{
    public sealed class StayNotFoundException( StayId stayId ):ApplicationException($"Stay '{stayId}' was not found.");
}

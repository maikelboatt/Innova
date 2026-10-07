using Innova.Application.Exceptions;
using Innova.Domain.FrontDesk.ValueObjects;

namespace Innova.Application.FrontDesk.Stay.Exceptions
{
    public sealed class StayNotFoundException( StayId stayId ):ApplicationExceptions($"Stay '{stayId}' was not found.");
}

using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.HouseKeeping.HouseKeeping.Commands.CompleteTask
{
    public sealed record CompleteTaskCommand( Guid HouseKeepingId, bool InspectionPassed, string? InspectionNotes ):ICommand<Guid>;
}

//
// Single, centralized authorization policy. A command type absent
// from this map is treated as "any authenticated user" — not
// "denied by default" — since retrofitting every existing command
// across five bounded contexts in one pass isn't realistic; commands
// are added here deliberately, as each one's access requirement is
// actually decided, rather than the whole app locking down at once.
// AuthorizationCommandHandler still requires SOME signed-in user
// regardless of whether a command appears here.

using Innova.Application.Billing.Folio.Commands.OpenFolio;
using Innova.Application.Billing.Folio.Commands.PostAdjustment;
using Innova.Application.Billing.Folio.Commands.PostCharge;
using Innova.Application.Billing.Folio.Commands.RecordPayment;
using Innova.Application.Billing.Folio.Commands.SettleFolio;
using Innova.Application.Billing.Folio.Commands.VoidFolio;
using Innova.Application.FrontDesk.Stay.Commands.AddOccupant;
using Innova.Application.FrontDesk.Stay.Commands.CheckIn;
using Innova.Application.FrontDesk.Stay.Commands.CheckOut;
using Innova.Application.GuestManagement.Guest.Commands.ChangeGuestIdentityDocument;
using Innova.Application.GuestManagement.Guest.Commands.DeactivateGuest;
using Innova.Application.GuestManagement.Guest.Commands.ReactivateGuest;
using Innova.Application.GuestManagement.Guest.Commands.RegisterGuest;
using Innova.Application.GuestManagement.Guest.Commands.UpdateGuestContactDetails;
using Innova.Application.HouseKeeping.HouseKeeping.Commands.AssignTask;
using Innova.Application.HouseKeeping.HouseKeeping.Commands.CompleteTask;
using Innova.Application.HouseKeeping.HouseKeeping.Commands.ScheduleTask;
using Innova.Application.Identity.Commands.DeactivateUser;
using Innova.Application.Identity.Commands.ReactivateUser;
using Innova.Application.Identity.Commands.RegisterUser;
using Innova.Application.Reservations.GroupBooking.Commands.AttachReservationToGroup;
using Innova.Application.Reservations.GroupBooking.Commands.DetachReservationFromGroup;
using Innova.Application.Reservations.GroupBooking.Commands.OpenGroupBooking;
using Innova.Application.Reservations.Reservation.Commands.BookReservation;
using Innova.Application.Reservations.Reservation.Commands.CancelReservation;
using Innova.Application.Reservations.Reservation.Commands.ConfirmReservation;
using Innova.Application.Reservations.Reservation.Commands.MarkNoShow;
using Innova.Application.RoomInventory.Room.Commands.CreateRoom;
using Innova.Application.RoomInventory.Room.Commands.ReturnRoomToService;
using Innova.Application.RoomInventory.Room.Commands.TakeRoomOutOfService;
using Innova.Application.RoomInventory.RoomTypeAllotment.Commands.CreateRoomTypeAllotment;
using Innova.Application.RoomInventory.RoomTypeDefinition.Commands.DefineRoomType;
using Innova.Application.RoomInventory.RoomTypeDefinition.Commands.UpdateRoomType;
using Innova.Domain.Identity.ValueObjects;

// ...existing usings, plus:

namespace Innova.Application.Authorization
{
    public static class CommandRoleRequirements
    {
        public static readonly IReadOnlyDictionary<Type, IReadOnlyCollection<Role>> Map =
            new Dictionary<Type, IReadOnlyCollection<Role>>
            {
                // Guest Management
                [typeof(RegisterGuestCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(UpdateGuestContactDetailsCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(ChangeGuestIdentityDocumentCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(DeactivateGuestCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(ReactivateGuestCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],

                // Room Inventory
                [typeof(CreateRoomCommand)] = [Role.Admin, Role.Manager],
                [typeof(ReturnRoomToServiceCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(TakeRoomOutOfServiceCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(DefineRoomTypeCommand)] = [Role.Admin, Role.Manager],
                [typeof(UpdateRoomTypeCommand)] = [Role.Admin, Role.Manager],
                [typeof(CreateRoomTypeAllotmentCommand)] = [Role.Admin, Role.Manager],

                // Reservations
                [typeof(BookReservationCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(ConfirmReservationCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(CancelReservationCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(MarkReservationNoShowCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(OpenGroupBookingCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(AttachReservationToGroupCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(DetachReservationFromGroupCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],

                // Front Desk
                [typeof(AddOccupantCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(CheckInCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],
                [typeof(CheckOutCommand)] = [Role.Admin, Role.Receptionist, Role.Manager],

                // HouseKeeping
                [typeof(ScheduleTaskCommand)] = [Role.Admin, Role.HousekeepingStaff, Role.Manager],
                [typeof(AssignTaskCommand)] = [Role.Admin, Role.HousekeepingStaff, Role.Manager],
                [typeof(CompleteTaskCommand)] = [Role.Admin, Role.HousekeepingStaff, Role.Manager],

                // Billing
                [typeof(OpenFolioCommand)] = [Role.Admin, Role.BillingStaff, Role.Manager],
                [typeof(PostChargeCommand)] = [Role.Admin, Role.BillingStaff, Role.Manager],
                [typeof(PostAdjustmentCommand)] = [Role.Admin, Role.BillingStaff, Role.Manager],
                [typeof(RecordPaymentCommand)] = [Role.Admin, Role.BillingStaff, Role.Manager],
                [typeof(SettleFolioCommand)] = [Role.Admin, Role.BillingStaff, Role.Manager],
                [typeof(VoidFolioCommand)] = [Role.Admin, Role.BillingStaff, Role.Manager],

                // Identity
                [typeof(RegisterUserCommand)] = [Role.Admin],
                [typeof(DeactivateUserCommand)] = [Role.Admin],
                [typeof(ReactivateUserCommand)] = [Role.Admin]
            };
    }
}

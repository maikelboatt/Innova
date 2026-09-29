// MediCore.Application/Authorization/CommandRoleRequirements.cs
//
// Single, centralized authorization policy. A command type absent
// from this map is treated as "any authenticated user" — not
// "denied by default" — since retrofitting every existing command
// across five bounded contexts in one pass isn't realistic; commands
// are added here deliberately, as each one's access requirement is
// actually decided, rather than the whole app locking down at once.
// AuthorizationCommandHandler still requires SOME signed-in user
// regardless of whether a command appears here.

using Innova.Domain.Identity.ValueObjects;

// ...existing usings, plus:

namespace Innova.Application.Authorization
{
    public static class CommandRoleRequirements
    {
        public static readonly IReadOnlyDictionary<Type, IReadOnlyCollection<Role>> Map =
            new Dictionary<Type, IReadOnlyCollection<Role>>
            {
                // // Patient Registration
                // [typeof(RegisterPatientCommand)] = [Role.Admin, Role.Receptionist],
                // [typeof(UpdatePatientCommand)] = [Role.Admin, Role.Receptionist],
                // [typeof(UnregisterPatientCommand)] = [Role.Admin, Role.Receptionist],
                // [typeof(RestorePatientCommand)] = [Role.Admin, Role.Receptionist],
                //
                // // Scheduling
                // [typeof(BookAppointmentCommand)] = [Role.Admin, Role.Receptionist],
                // [typeof(RescheduleAppointmentCommand)] = [Role.Admin, Role.Receptionist],
                // [typeof(CancelAppointmentCommand)] = [Role.Admin, Role.Receptionist],
                // [typeof(MarkAppointmentNoShowCommand)] = [Role.Admin, Role.Receptionist],
                // [typeof(CompleteAppointmentCommand)] = [Role.Admin, Role.Doctor],
                //
                // // Consultation
                // [typeof(OpenConsultationCommand)] = [Role.Admin, Role.Doctor],
                // [typeof(StartConsultationCommand)] = [Role.Admin, Role.Doctor],
                // [typeof(CompleteConsultationCommand)] = [Role.Admin, Role.Doctor],
                // [typeof(CancelConsultationCommand)] = [Role.Admin, Role.Doctor],
                // [typeof(UpdateClinicalNotesCommand)] = [Role.Admin, Role.Doctor],
                //
                // // Pharmacy
                // [typeof(IssuePrescriptionCommand)] = [Role.Admin, Role.Doctor],
                // [typeof(ValidatePrescriptionCommand)] = [Role.Admin, Role.Pharmacist],
                // [typeof(BeginDispensingCommand)] = [Role.Admin, Role.Pharmacist],
                // [typeof(DispenseMedicationCommand)] = [Role.Admin, Role.Pharmacist],
                // [typeof(FulfillPrescriptionCommand)] = [Role.Admin, Role.Pharmacist],
                // [typeof(RejectPrescriptionCommand)] = [Role.Admin, Role.Pharmacist],
                // [typeof(UpdateDrugPriceCommand)] = [Role.Admin],
                //
                // // Billing 
                // [typeof(IssueBillCommand)] = [Role.Admin, Role.BillingStaff],
                // [typeof(AddStandardPaymentCommand)] = [Role.Admin, Role.BillingStaff],
                // [typeof(CancelBillCommand)] = [Role.Admin, Role.BillingStaff],
                //
                // // Identity
                // [typeof(RegisterUserCommand)] = [Role.Admin],
                // [typeof(DeactivateUserCommand)] = [Role.Admin],
                // [typeof(ReactivateUserCommand)] = [Role.Admin]
            };
    }
}

using Innova.Domain.Common;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.Events;
using Innova.Domain.Reservations.ValueObjects;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.Exceptions;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.Reservations.Aggregates
{
    public sealed class Reservation:AggregateRoot<ReservationId>
    {
        private Reservation( ReservationId reservationId,
                             GuestId guestId,
                             GroupBookingId? groupBookingId,
                             RoomTypeId roomTypeRequested,
                             DateRange stayPeriod,
                             RatePlan ratePlan,
                             ReservationStatus status ):base(reservationId)
        {
            GuestId = guestId;
            GroupBookingId = groupBookingId;
            RoomTypeRequested = roomTypeRequested;
            StayPeriod = stayPeriod;
            RatePlan = ratePlan;
            Status = status;
        }

        public GuestId GuestId { get; }
        public GroupBookingId? GroupBookingId { get; }
        public RoomTypeId RoomTypeRequested { get; }
        public DateRange StayPeriod { get; }
        public RatePlan RatePlan { get; }
        public ReservationStatus Status { get; private set; }

        public static Reservation Booked( GuestId guestId,
                                          RoomTypeId roomTypeRequested,
                                          DateRange stayPeriod,
                                          RatePlan ratePlan,
                                          GroupBookingId? groupBookingId = null )
        {
            ArgumentNullException.ThrowIfNull(guestId);
            ArgumentNullException.ThrowIfNull(roomTypeRequested);
            ArgumentNullException.ThrowIfNull(stayPeriod);

            ReservationId reservationId = ReservationId.New();

            Reservation reservation = new(
                reservationId,
                guestId,
                groupBookingId,
                roomTypeRequested,
                stayPeriod,
                ratePlan,
                ReservationStatus.Tentative);

            reservation.RaiseDomainEvent(
                new ReservationBooked(
                    reservationId.Value,
                    guestId.Value,
                    groupBookingId?.Value,
                    roomTypeRequested.Value,
                    stayPeriod.Start,
                    stayPeriod.End,
                    ratePlan.NightlyRate.Amount,
                    ratePlan.NightlyRate.Currency));

            return reservation;
        }

        public static Reservation Reconstitution( ReservationId reservationId,
                                                  GuestId guestId,
                                                  GroupBookingId groupBookingId,
                                                  RoomTypeId roomTypeRequested,
                                                  DateRange stayPeriod,
                                                  RatePlan ratePlan,
                                                  ReservationStatus status ) => new(
            reservationId,
            guestId,
            groupBookingId,
            roomTypeRequested,
            stayPeriod,
            ratePlan,
            status);


        public void Confirm( bool hasGuarantee )
        {
            if (!Status.CanTransitionTo(ReservationStatus.Confirmed))
                throw new DomainException($"Cannot confirm a reservation is status {Status}");

            if (!hasGuarantee)
                throw new DomainException("Confirming a reservation requires a deposit or a card guarantee");

            Status = ReservationStatus.Confirmed;

            RaiseDomainEvent(
                new ReservationConfirmed(
                    Id.Value,
                    GuestId.Value,
                    GroupBookingId?.Value,
                    RoomTypeRequested.Value,
                    StayPeriod.Start,
                    StayPeriod.End,
                    RatePlan.NightlyRate.Amount,
                    RatePlan.NightlyRate.Currency,
                    DateTime.UtcNow));
        }

        public void MarkCheckedIn()
        {
            if (!Status.CanTransitionTo(ReservationStatus.CheckedIn))
                throw new DomainException($"Only a Confirmed reservation can be checked in (was {Status})");

            Status = ReservationStatus.CheckedIn;

            RaiseDomainEvent(
                new ReservationCheckedIn(
                    Id.Value,
                    GuestId.Value,
                    GroupBookingId?.Value,
                    RoomTypeRequested.Value,
                    StayPeriod.Start,
                    StayPeriod.End,
                    RatePlan.NightlyRate.Amount,
                    RatePlan.NightlyRate.Currency,
                    DateTime.UtcNow));
        }

        public void MarkCheckedOut()
        {
            if (!Status.CanTransitionTo(ReservationStatus.CheckedOut))
                throw new DomainException($"Only a CheckedIn reservation can be checked out (was {Status})");

            Status = ReservationStatus.CheckedOut;

            RaiseDomainEvent(
                new ReservationCheckedOut(
                    Id.Value,
                    GuestId.Value,
                    GroupBookingId?.Value,
                    RoomTypeRequested.Value,
                    StayPeriod.Start,
                    StayPeriod.End,
                    RatePlan.NightlyRate.Amount,
                    RatePlan.NightlyRate.Currency,
                    DateTime.UtcNow));
        }

        public void Cancel()
        {
            if (!Status.CanTransitionTo(ReservationStatus.Cancelled))
                throw new DomainException($"Cannot cancel a reservation in status {Status}");

            DateTime cancelledAt = DateTime.UtcNow;
            Money cancellationFee = RatePlan.CancellationPolicy.FeeFor(cancelledAt, StayPeriod.Start);

            Status = ReservationStatus.Cancelled;

            RaiseDomainEvent(
                new ReservationCancelled(
                    Id.Value,
                    GuestId.Value,
                    GroupBookingId?.Value,
                    RoomTypeRequested.Value,
                    StayPeriod.Start,
                    StayPeriod.End,
                    RatePlan.NightlyRate.Amount,
                    RatePlan.NightlyRate.Currency,
                    cancellationFee.Amount,
                    cancellationFee.Currency, // new fields
                    cancelledAt));
        }

        public void MarkNoShow()
        {
            if (!Status.CanTransitionTo(ReservationStatus.NoShow))
                throw new DomainException($"Only a confirmed reservation can be marked No-Show (was {Status})");

            Status = ReservationStatus.NoShow;

            RaiseDomainEvent(
                new ReservationMarkedNoShow(
                    Id.Value,
                    GuestId.Value,
                    GroupBookingId?.Value,
                    RoomTypeRequested.Value,
                    StayPeriod.Start,
                    StayPeriod.End,
                    RatePlan.NightlyRate.Amount,
                    RatePlan.NightlyRate.Currency,
                    DateTime.UtcNow));
        }
    }
}

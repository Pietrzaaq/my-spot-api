﻿using MySpot.Api.Exceptions;
using MySpot.Api.ValueObjects;

namespace MySpot.Api.Entities;

public class WeeklyParkingSpot
{
    private readonly HashSet<Reservation> _reservations = new();

    public ParkingSpotId Id { get; }
    public Week Week { get; }
    public string Name { get; } 
    public IEnumerable<Reservation> Reservations => _reservations;

    public WeeklyParkingSpot(Guid id, Week week, string name)
    {
        Id = id;
        Week = week;
        Name = name;
    }

    public void AddReservation(Reservation reservation, Date now)
    {
        var isInvalidDate = reservation.Date < Week.From ||
                            reservation.Date > Week.To ||
                            reservation.Date <= now; 
        if (isInvalidDate)
        {
            throw new InvalidReservationDateException(reservation.Date.Value);
        }

        var reservationAlreadyExists = Reservations.Any(x =>
            x.Date.Value.Date == reservation.Date.Value.Date);
        if (reservationAlreadyExists)
        {
            throw new ParkingSpotAlreadyReservedException(Name, reservation.Date.Value.Date);
        }

        _reservations.Add(reservation);
    }

    public void RemoveReservation(ReservationId reservationId)
    {
        var existingReservation = _reservations.FirstOrDefault(x => x.Id == reservationId);

        if (existingReservation is null)
        {
            return;
        }
        
        _reservations.Remove(existingReservation);
    }
}
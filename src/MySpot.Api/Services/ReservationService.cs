﻿using MySpot.Api.Commands;
using MySpot.Api.DTO;
using MySpot.Api.Entities;
using MySpot.Api.ValueObjects;

namespace MySpot.Api.Services;

public class ReservationService
{
    private static Clock Clock = new();
    private readonly List<WeeklyParkingSpot> _weeklyParkingSpots;
    
    public ReservationService(List<WeeklyParkingSpot> weeklyParkingSpots)
    {
        _weeklyParkingSpots = weeklyParkingSpots;
    }

    public ReservationDto Get(Guid id)
        => GetAllWeekly().SingleOrDefault(x => x.Id == id);
    
    public IEnumerable<ReservationDto> GetAllWeekly() 
        => _weeklyParkingSpots.SelectMany(x => x.Reservations)
            .Select(x => new ReservationDto
            {
                Id = x.Id,
                ParkingSpotId = x.ParkingSpotId,
                EmployeeName = x.EmployeeName,
                Date = x.Date.Value.Date   
            });
    

    public Guid? Create(CreateReservation command)
    {
        var weeklyParkingSpot = _weeklyParkingSpots.SingleOrDefault(x => x.Id.Value == command.ParkingSpotId);
        if (weeklyParkingSpot is null)
        {
            return default;
        }

        var reservation = new Reservation(
            command.ReservationId,
            command.ParkingSpotId,
            command.EmployeeName,
            command.LicensePlate,
            new Date(command.Date));
            
        weeklyParkingSpot.AddReservation(reservation, new Date(Clock.Current()));

        return command.ReservationId;
    }

    public bool Update(ChangeReservationLicensePlate command)
    {
        var weeklyParkingSpot = GetWeeklyParkingSpotByReservation(command.ReservationId);
        if (weeklyParkingSpot is null)
        {
            return false;
        }
    
        var existingReservation = weeklyParkingSpot.Reservations.SingleOrDefault(x => x.Id.Value == command.ReservationId);
        if (existingReservation is null)
        {
            return false;
        }

        if (existingReservation.Date <= new Date(Clock.Current()))
        {
            return false;
        }
        
        existingReservation.ChangeLicensePlate(command.LicensePlate);
        
        return true;
    }

    public bool Delete(DeleteReservation command)
    {
        var weeklyParkingSpot = GetWeeklyParkingSpotByReservation(command.ReservationId);
        if (weeklyParkingSpot is null)
        {
            return false;
        }
        
        var existingReservation = weeklyParkingSpot.Reservations.SingleOrDefault(x => x.Id.Value == command.ReservationId);
        if (existingReservation is null)
        {
            return default;
        }

        weeklyParkingSpot.RemoveReservation(command.ReservationId);
        
        return true;  
    }

    private WeeklyParkingSpot GetWeeklyParkingSpotByReservation(Guid reservationId)
        => _weeklyParkingSpots.SingleOrDefault(x => x.Reservations.Any(x => x.Id.Value == reservationId));
}
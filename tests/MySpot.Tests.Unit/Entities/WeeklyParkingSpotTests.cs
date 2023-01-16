using System;
using MySpot.Api.Entities;
using MySpot.Api.Exceptions;
using MySpot.Api.ValueObjects;
using Shouldly;
using Xunit;

namespace MySpot.Tests.Unit.Entities;

public class WeeklyParkingSpotTests
{
     [Theory]
     [InlineData("2023-01-08")]
     [InlineData("2023-01-16")]
     public void given_invalid_date_add_reservation_should_fail(string dateString)
     {
          // Arrange
          var invalidDate = DateTime.Parse(dateString);
          var reservation = new Reservation(Guid.NewGuid(), _weeklyParkingSpot.Id, "John Doe",
               "XYZ1234", new Date(invalidDate));

          // Act
          var exception = Record.Exception(() => _weeklyParkingSpot.AddReservation(reservation, new Date(_now)));

          // Assert
          exception.ShouldNotBeNull();
          exception.ShouldBeOfType<InvalidReservationDateException>();
     }

     [Fact]
     public void given_reservation_for_already_existing_date_add_reservation_should_fail()
     {
          var reservation = new Reservation(Guid.NewGuid(), _weeklyParkingSpot.Id, "John Doe",
               "XYZ123", _now.AddDays(1));
          _weeklyParkingSpot.AddReservation(reservation, _now);

          var exception = Record.Exception(() => _weeklyParkingSpot.AddReservation(reservation, _now));
          
          exception.ShouldNotBeNull();
          exception.ShouldBeOfType<ParkingSpotAlreadyReservedException>();
     }

    [Fact]
    public void given_reservation_for_not_taken_date_add_reservation_should_succeed()
     {
          var reservation = new Reservation(Guid.NewGuid(), _weeklyParkingSpot.Id, "John Doe",
               "XYZ123", _now.AddDays(1));
          _weeklyParkingSpot.AddReservation(reservation, _now);

          _weeklyParkingSpot.Reservations.ShouldHaveSingleItem();
     }

     #region Arrange
     
     private readonly Date _now;
     private readonly WeeklyParkingSpot _weeklyParkingSpot;

     public WeeklyParkingSpotTests()
     {
          _now = new Date(new DateTime(2023, 01, 15)).AddDays(-2);
          _weeklyParkingSpot = new WeeklyParkingSpot(Guid.NewGuid(), new Week(_now), "P1");
     }
     
     #endregion
}
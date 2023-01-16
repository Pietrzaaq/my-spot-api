namespace MySpot.Api.Exceptions;

public sealed class InvalidReservationDateException : CustomException
{
    public DateTimeOffset Date { get; }
    
    public InvalidReservationDateException(DateTimeOffset date)
        : base($"Reservation date: {date:d} is invalid.")
    {
        Date = date;
    }
}
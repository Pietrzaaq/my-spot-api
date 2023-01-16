﻿using MySpot.Api.Exceptions;

namespace MySpot.Api.ValueObjects;

public sealed record ParkingSpotName(string Value)
{
    public string Value { get; } = Value ?? throw new InvalidParkingSpotNameException();

    public static implicit operator ParkingSpotName(string value) => new ParkingSpotName(value);
    public static implicit operator string(ParkingSpotName employeeName) => employeeName.Value;
}
using System;
using FlightDispatcher.Models;

namespace FlightDispatcher.Events;

public class LandedEventArgs : FlightEventArgs
{
    public string LandedAt { get;}
    public DateTime ArrivalTime { get; }
    
    public LandedEventArgs(string flightId, string landedAt, DateTime arrivalTime) : base(flightId)
    {
        LandedAt = landedAt;
        ArrivalTime = arrivalTime;
    }
}
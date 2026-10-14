using System;
using FlightDispatcher.Models;

namespace FlightDispatcher.Events;

public class LandedEventArgs : FlightEventArgs
{
    public string LandedIn { get;}
    public DateTime ArrivalTime { get; }
    
    public LandedEventArgs(string flightId, string airline, string landedIn, DateTime arrivalTime) : base(flightId, airline)
    {
        LandedIn = landedIn;
        ArrivalTime = arrivalTime;
    }
}
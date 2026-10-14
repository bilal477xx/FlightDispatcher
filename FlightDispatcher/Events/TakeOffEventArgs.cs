using System;
using FlightDispatcher.Models;

namespace FlightDispatcher.Events;

public class TakeOffEventArgs : FlightEventArgs
{
    DateTime DepartureTime { get; }
    
    public TakeOffEventArgs(string flightId, DateTime departureTime) : base(flightId)
    {
        DepartureTime = departureTime;
    }
}
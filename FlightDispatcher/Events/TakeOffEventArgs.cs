using System;
using FlightDispatcher.Models;

namespace FlightDispatcher.Events;

public class TakeOffEventArgs : FlightEventArgs
{
    public string Destination { get; }
    public DateTime DepartureTime { get; }
    
    public TakeOffEventArgs(string flightId, string airline, string destination, DateTime departureTime) : base(flightId, airline)
    {
        Destination = destination;
        DepartureTime = departureTime;
    }
}
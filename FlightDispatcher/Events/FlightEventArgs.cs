using System;
using FlightDispatcher.Models;

namespace FlightDispatcher.Events;

public class FlightEventArgs : EventArgs
{
    public string FlightId  { get; }

    public FlightEventArgs(string flightId)
    {
        FlightId = flightId;
    }
}